using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Testing.Platform.Capabilities.TestFramework;
using Microsoft.Testing.Platform.Extensions.Messages;
using Microsoft.Testing.Platform.Extensions.TestFramework;
using Microsoft.Testing.Platform.Messages;
using Microsoft.Testing.Platform.Requests;
using Microsoft.Testing.Platform.Services;

namespace Prova
{
#pragma warning disable TPEXP // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
    /// <summary>
    /// A hybrid adapter that enables Prova tests to run on the Microsoft Testing Platform (MTP).
    /// </summary>
    public sealed class HybridMtpAdapter : ITestFramework, IDataProducer
    {
        private readonly IEnumerable<ProvaTest> _tests;
        private readonly ITestFrameworkCapabilities _capabilities;
        private readonly Configuration.ProvaConfig _config;

        /// <summary>
        /// Initializes a new instance of the <see cref="HybridMtpAdapter"/> class.
        /// </summary>
        /// <param name="tests">The collection of Prova tests to execute.</param>
        /// <param name="capabilities">The test framework capabilities.</param>
        public HybridMtpAdapter(IEnumerable<ProvaTest> tests, ITestFrameworkCapabilities capabilities)
        {
            _tests = tests;
            _capabilities = capabilities;
            _config = Configuration.ConfigLoader.Load();

            // Merge Global Properties from config
            foreach (var test in _tests)
            {
                foreach (var prop in _config.GlobalProperties)
                {
                    if (!test.Properties.ContainsKey(prop.Key)) test.Properties[prop.Key] = prop.Value;
                }
            }
        }

        /// <inheritdoc />
        public string Uid => "Prova";
        /// <inheritdoc />
        public string Version => "0.5.0";
        /// <inheritdoc />
        public string DisplayName => "Prova";
        /// <inheritdoc />
        public string Description => "MTP-Native, Zero-Reflection Testing for .NET 10";

        /// <inheritdoc />
        public Type[] DataTypesProduced => new[] { typeof(TestNodeUpdateMessage) };

        /// <inheritdoc />
        public Task<bool> IsEnabledAsync() => Task.FromResult(true);

        /// <inheritdoc />
        public Task<CreateTestSessionResult> CreateTestSessionAsync(CreateTestSessionContext context)
            => Task.FromResult(new CreateTestSessionResult { IsSuccess = true });

        /// <inheritdoc />
        public Task<CloseTestSessionResult> CloseTestSessionAsync(CloseTestSessionContext context)
            => Task.FromResult(new CloseTestSessionResult { IsSuccess = true });

        /// <inheritdoc />
        public async Task ExecuteRequestAsync(ExecuteRequestContext context)
        {
            if (context.Request is DiscoverTestExecutionRequest discoverRequest)
            {
                foreach (var test in _tests)
                {
                    var node = MapToNode(test);
                    node.Properties.Add(DiscoveredTestNodeStateProperty.CachedInstance);
                    await context.MessageBus.PublishAsync(this, new TestNodeUpdateMessage(discoverRequest.Session.SessionUid, node));
                }
                context.Complete();
                return;
            }

            if (context.Request is RunTestExecutionRequest runRequest)
            {
                var session = runRequest.Session;
                var messageBus = context.MessageBus;

                // Bounded Parallelism (CRITICAL)
                int? specMax = _tests.Select(t => t.MaxParallel).Where(m => m.HasValue).Min();
                int maxParallel = _config.MaxParallel ?? specMax ?? Environment.ProcessorCount;

                using var semaphore = new SemaphoreSlim(maxParallel);

                // Isolation primitives. Previously this adapter honoured only the global
                // concurrency limit, so [DoNotParallelize], [NotInParallel(...)] and
                // [ParallelLimiter(...)] were silently ignored whenever tests ran through
                // `dotnet test` / Microsoft.Testing.Platform - which is the documented and
                // CI path. Tests that declared they needed isolation still ran concurrently,
                // producing intermittent failures and corrupting shared state. The standalone
                // generated runner already implemented these; the semantics are mirrored here.
                using var exclusiveLock = new SemaphoreSlim(1, 1);
                var resourceSemaphores = new ConcurrentDictionary<string, SemaphoreSlim>();

                var tasks = new List<Task>();

                foreach (var test in _tests)
                {
                    if (test.DoNotParallelize)
                    {
                        // Drain everything already scheduled so this test truly runs alone.
                        await Task.WhenAll(tasks).ConfigureAwait(false);
                        tasks.Clear();
                    }

                    await semaphore.WaitAsync(context.CancellationToken);

                    tasks.Add(Task.Run(async () =>
                    {
                        var heldResources = new List<SemaphoreSlim>();
                        var hasExclusiveLock = false;
                        try
                        {
                            if (test.DoNotParallelize)
                            {
                                await exclusiveLock.WaitAsync(context.CancellationToken).ConfigureAwait(false);
                                hasExclusiveLock = true;
                            }
                            else
                            {
                                // Resource constraints are acquired in a stable order to
                                // avoid deadlock between tests holding overlapping keys.
                                if (test.ResourceConstraints is { Count: > 0 })
                                {
                                    foreach (var res in test.ResourceConstraints.Where(static x => x != null).OrderBy(static x => x, StringComparer.Ordinal))
                                    {
                                        var sem = resourceSemaphores.GetOrAdd(res!, static _ => new SemaphoreSlim(1, 1));
                                        await sem.WaitAsync(context.CancellationToken).ConfigureAwait(false);
                                        heldResources.Add(sem);
                                    }
                                }

                                if (test.ParallelLimiters is { Count: > 0 })
                                {
                                    foreach (var (key, limit) in test.ParallelLimiters.OrderBy(static x => x.Key, StringComparer.Ordinal))
                                    {
                                        var sem = resourceSemaphores.GetOrAdd(key, _ => new SemaphoreSlim(limit, limit));
                                        await sem.WaitAsync(context.CancellationToken).ConfigureAwait(false);
                                        heldResources.Add(sem);
                                    }
                                }
                            }

                            if (test.SkipReason != null)
                            {
                                await EventRegistry.DispatchEndAsync(test, TestResult.Skipped, 0);
                                await ReportSkippedAsync(messageBus, session.SessionUid, test);
                            }
                            else
                            {
                                await RunTestAsync(messageBus, session.SessionUid, test, context.CancellationToken);
                            }
                        }
                        finally
                        {
                            for (int i = heldResources.Count - 1; i >= 0; i--)
                            {
                                heldResources[i].Release();
                            }

                            if (hasExclusiveLock)
                            {
                                exclusiveLock.Release();
                            }

                            semaphore.Release();
                        }
                    }, context.CancellationToken));

                    if (test.DoNotParallelize)
                    {
                        // Let the isolated test complete before scheduling anything else.
                        await Task.WhenAll(tasks).ConfigureAwait(false);
                        tasks.Clear();
                    }
                }

                await Task.WhenAll(tasks);
                context.Complete();
            }
        }

        private async Task RunTestAsync(IMessageBus messageBus, Microsoft.Testing.Platform.TestHost.SessionUid sessionUid, ProvaTest test, CancellationToken ct)
        {
            var sw = Stopwatch.StartNew();
            await EventRegistry.DispatchStartAsync(test);

            // Report InProgress
            var inProgressNode = MapToNode(test);
            inProgressNode.Properties.Add(InProgressTestNodeStateProperty.CachedInstance);
            await messageBus.PublishAsync(this, new TestNodeUpdateMessage(sessionUid, inProgressNode));

            int attempts = 0;
            int maxAttempts = (test.RetryCount ?? _config.DefaultRetryCount ?? 0) + 1;
            Exception? lastException = null;

            while (attempts < maxAttempts)
            {
                attempts++;
                using var cts = CancellationTokenSource.CreateLinkedTokenSource(ct);
                var testContext = new TestContext(test.DisplayName, test.Properties, cts.Token);
                TestContext.Current = testContext;
                try
                {
                    string? output = await test.ExecuteDelegate();

                    sw.Stop();
                    var passedNode = MapToNode(test);
                    passedNode.Properties.Add(PassedTestNodeStateProperty.CachedInstance);
                    passedNode.Properties.Add(new TimingProperty(new TimingInfo(DateTimeOffset.Now - sw.Elapsed, DateTimeOffset.Now, sw.Elapsed)));

                    if (!string.IsNullOrEmpty(output))
                    {
                        passedNode.Properties.Add(new StandardOutputProperty(output));
                    }

                    await messageBus.PublishAsync(this, new TestNodeUpdateMessage(sessionUid, passedNode));
                    await EventRegistry.DispatchEndAsync(test, TestResult.Passed, sw.ElapsedMilliseconds);
                    return;
                }
                catch (Exception ex)
                {
                    lastException = ex;
                    if (attempts >= maxAttempts)
                    {
                        sw.Stop();
                        var failedNode = MapToNode(test);
                        failedNode.Properties.Add(new FailedTestNodeStateProperty(ex));
                        failedNode.Properties.Add(new TimingProperty(new TimingInfo(DateTimeOffset.Now - sw.Elapsed, DateTimeOffset.Now, sw.Elapsed)));
                        await messageBus.PublishAsync(this, new TestNodeUpdateMessage(sessionUid, failedNode));
                        await EventRegistry.DispatchEndAsync(test, TestResult.Failed, sw.ElapsedMilliseconds);
                    }
                }
                finally
                {
                    TestContext.Current = null!;
                }
            }
        }

        private async Task ReportSkippedAsync(IMessageBus messageBus, Microsoft.Testing.Platform.TestHost.SessionUid sessionUid, ProvaTest test)
        {
            var node = MapToNode(test);
            node.Properties.Add(new SkippedTestNodeStateProperty(test.SkipReason ?? "Skipped"));
            await messageBus.PublishAsync(this, new TestNodeUpdateMessage(sessionUid, node));
        }

        private static TestNode MapToNode(ProvaTest test)
        {
            var node = new TestNode
            {
                Uid = new Microsoft.Testing.Platform.Extensions.Messages.TestNodeUid(test.DisplayName),
                DisplayName = test.DisplayName,
                Properties = new PropertyBag()
            };

            foreach (var prop in test.Properties)
            {
                // In MTP 2.0.2, KeyValuePairStringProperty is replaced by TestMetadataProperty
                node.Properties.Add(new Microsoft.Testing.Platform.Extensions.Messages.TestMetadataProperty(prop.Key, prop.Value));
            }

            return node;
        }
    }
}
