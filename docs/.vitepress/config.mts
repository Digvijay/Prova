import { defineConfig } from 'vitepress'

export default defineConfig({
    base: '/',
    title: "Prova",
    description: "High-Performance AOT Test Runner for .NET",
    head: [
        ['link', { rel: 'icon', href: '/icon.png' }]
    ],
    themeConfig: {
        // Logo if you have one, or just text
        // logo: '/logo.png', 

        nav: [
            { text: 'Home', link: '/' },
            { text: 'Guide', link: '/guide/getting-started' },
            { text: 'Features', link: '/features/parallelism' },
            { text: 'Integrations', link: '/integrations/aspnet-core' },
            { text: 'Reference', link: '/api/index' }
        ],

        sidebar: [
            {
                text: 'Introduction',
                items: [
                    { text: '🚀 Getting Started', link: '/guide/getting-started' },
                    { text: '🏗️ Architecture', link: '/concepts/architecture' },
                    { text: '📄 Migration Guide', link: '/guide/migration' },
                    { text: '⚔️ Prova vs Others', link: '/guide/comparison' },
                    { text: '🛣️ Roadmap', link: '/roadmap' }
                ]
            },
            {
                text: 'Writing Tests',
                items: [
                    { text: '✍️ Basics', link: '/guide/writing-tests' },
                    { text: '🏃 Running Tests', link: '/guide/running-tests' },
                    { text: '🚀 CLI Reference', link: '/guide/command-line' },
                    { text: '📊 Data-Driven', link: '/features/data-driven-testing' },
                    { text: '🧮 Combinatorial ([Matrix])', link: '/features/combinatorial' },
                    { text: '💉 Data Sources', link: '/features/data-sources' },
                    { text: '🛠️ Data Generators', link: '/features/data-generators' },
                    { text: '🏭 Class Factories', link: '/features/class-factories' },
                    { text: '🧬 Generic Tests', link: '/features/generic-tests' },
                    { text: '🔀 Test Variants', link: '/features/test-variants' },
                    { text: '🎭 Dynamic Tests', link: '/features/dynamic-tests' },
                    { text: '🏷️ Display Names', link: '/features/display-names' },
                    { text: '🎨 Argument Formatting', link: '/features/argument-formatting' }
                ]
            },
            {
                text: 'Advanced Features',
                items: [
                    { text: '⚡ Parallelism', link: '/features/parallelism' },
                    { text: '🔄 Advanced Parallelism', link: '/features/advanced-parallelism' },
                    { text: '🚦 Resource Constraints', link: '/features/resource-constraints' },
                    { text: '⏱️ Timeouts', link: '/features/timeouts' },
                    { text: '🔁 Flow Control', link: '/features/flow-control' },
                    { text: '🔄 Lifecycle Hooks', link: '/features/lifecycle-hooks' },
                    { text: '🌐 Global Hooks', link: '/features/global-hooks' },
                    { text: '🌍 Global Policies', link: '/features/global-policies' },
                    { text: '🌐 Test Context', link: '/api/test-context-ref' },
                    { text: '📜 Logging', link: '/features/logging' },
                    { text: '🔔 Test Events', link: '/features/events' },
                    { text: '📊 Code Coverage', link: '/features/coverage' },
                    { text: '⚙️ Custom Executors', link: '/features/custom-executors' },
                    { text: '🔍 Debugging', link: '/features/debugging' },
                    { text: '📂 Artifacts & Observability', link: '/features/artifacts' },
                    { text: '📦 State Sharing', link: '/features/state-sharing' },
                    { text: '📜 Scripting', link: '/features/scripting' }
                ]
            },
            {
                text: 'Integrations',
                items: [
                    { text: '🌐 ASP.NET Core', link: '/integrations/aspnet-core' },
                    { text: '🎭 Playwright', link: '/integrations/playwright' },
                    { text: '🐳 Testcontainers', link: '/integrations/testcontainers' },
                    { text: '🎲 FsCheck (PBT)', link: '/integrations/fscheck' },
                    { text: '☁️ Skugga', link: '/integrations/skugga' }
                ]
            },
            {
                text: 'API Reference',
                items: [
                    { text: '🏷️ Attributes', link: '/api/attributes' },
                    { text: '✅ Assertions', link: '/api/assertions' },
                    { text: '⚙️ Configuration', link: '/guide/configuration-json' },
                    { text: '📖 API Overview', link: '/api/overview' }
                ]
            },
            {
                text: 'Concepts',
                items: [
                    { text: '🛡️ Bounded Parallelism', link: '/concepts/parallelism' },
                    { text: '💉 Dependency Injection', link: '/concepts/di' },
                    { text: '🧩 Hybrid MTP', link: '/concepts/mtp' },
                    { text: '📄 Technical Summary', link: '/concepts/technical-summary' },
                    { text: '📄 Executive Summary', link: '/concepts/executive-summary' }
                ]
            },
            {
                text: 'Resources',
                items: [
                    { text: '💡 Best Practices', link: '/guide/best-practices' },
                    { text: '🚀 Performance', link: '/guide/performance' },
                    { text: '🏗️ Orchestration', link: '/guide/orchestration' }
                ]
            }
        ],

        socialLinks: [
            { icon: 'github', link: 'https://github.com/Digvijay/Prova' }
        ],

        footer: {
            message: 'Released under the MIT License.',
            copyright: 'Copyright © 2026 Digvijay'
        }
    }
})
