using System;
using System.Threading;
using System.Threading.Tasks;
using Prova;

namespace Prova.Demo
{
    public class StaExecutor : ITestExecutor
    {
        public async Task<string?> ExecuteAsync(Func<Task<string?>> testAction, ProvaTest testInfo)
        {
            TaskCompletionSource<string?> tcs = new TaskCompletionSource<string?>();
            Thread thread = new Thread(async () =>
            {
                try
                {
                    string? result = await testAction();
                    tcs.SetResult(result);
                }
                catch (Exception ex)
                {
                    tcs.SetException(ex);
                }
            });

            if (global::System.OperatingSystem.IsWindows())
            {
                thread.SetApartmentState(ApartmentState.STA);
            }
            thread.Start();

            return await tcs.Task;
        }
    }
}
