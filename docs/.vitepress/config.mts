import { defineConfig } from 'vitepress'

export default defineConfig({
    base: '/Prova/',
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
            { text: 'Reference', link: '/api/attributes' }
        ],

        sidebar: [
            {
                text: 'Introduction',
                items: [
                    { text: '🏗️ Architecture', link: '/guide/why-prova' },
                    { text: '🚀 Getting Started', link: '/guide/getting-started' },
                    { text: '📄 Migration Guide', link: '/guide/migration' },
                    { text: '✨ Advanced Features', link: '/guide/features' },
                    { text: '🧬 Generic Tests', link: '/features/generic-tests' },
                    { text: '⏱️ Timeouts', link: '/guide/timeouts' },
                    { text: '📊 Code Coverage', link: '/guide/coverage' },
                    { text: '🔁 Flow Control', link: '/guide/flow-control' },
                    { text: '🚦 Resource Constraints', link: '/guide/resource-constraints' },
                    { text: '🔄 Lifecycle Hooks', link: '/guide/lifecycle-hooks' },
                    { text: '🌐 Global Hooks', link: '/guide/global-hooks' },
                    { text: '🔍 Debugging', link: '/debugging' },
                    { text: '🛣️ Roadmap (v0.4.0)', link: '/roadmap' }
                ]
            },
            {
                text: 'Core Concepts',
                items: [
                    { text: '🛡️ Bounded Parallelism', link: '/concepts/parallelism' },
                    { text: '⚡ Parallel & Sequential', link: '/guide/sequential-parallel' },
                    { text: '📊 Data-Driven Testing', link: '/guide/data-driven-testing' },
                    { text: '💉 Dependency Injection', link: '/concepts/di' },
                    { text: '🧩 Hybrid MTP', link: '/concepts/mtp' },
                    { text: '🚦 Resource Constraints', link: '/guide/resource-constraints' },
                    { text: '🔄 Advanced Parallelism', link: '/guide/advanced-parallelism' },
                    { text: '🌍 Global Policies', link: '/guide/global-policies' },
                    { text: '🏷️ Properties & Filters', link: '/guide/properties-filters' },
                    { text: '💉 Data Sources', link: '/guide/data-sources' },
                    { text: '💉 DI Data Sources', link: '/features/di-data-sources' },
                    { text: '🛠️ Data Generators', link: '/guide/data-generators' },
                    { text: '🏭 Class Factories', link: '/guide/class-factories' },
                    { text: '⚙️ Runtime Config (JSON)', link: '/guide/configuration-json' },
                    { text: '🎭 Dynamic Tests', link: '/guide/dynamic-tests' },
                    { text: '🔀 Test Variants', link: '/features/test-variants' },
                    { text: '📜 Logging', link: '/features/logging' },
                    { text: '⚙️ Custom Executors', link: '/guide/custom-executors' },
                    { text: '🌐 Test Context', link: '/guide/test-context' },
                    { text: '🌐 ASP.NET Core', link: '/features/aspnet-core' },
                    { text: '🔔 Test Events', link: '/guide/events' },
                    { text: '🎭 Playwright', link: '/playwright' },
                    { text: '🐳 Testcontainers', link: '/testcontainers' },
                    { text: '🎲 FsCheck (PBT)', link: '/fscheck' },
                    { text: '🧮 Combinatorial ([Matrix])', link: '/guide/combinatorial' },
                    { text: '🏷️ Display Names', link: '/guide/display-names' },
                    { text: '🎨 Argument Formatting', link: '/guide/argument-formatting' },
                    { text: '🔍 Debugging', link: '/debugging' },
                    { text: '📈 Code Coverage', link: '/guide/coverage' },
                    { text: '📂 Artifacts & Observability', link: '/features/artifacts' }
                ]
            },
            {
                text: 'API Reference',
                items: [
                    { text: '🏷️ Attributes', link: '/api/attributes' },
                    { text: '⚙️ Configuration', link: '/guide/configuration' },
                    { text: '✅ Assertions', link: '/api/assertions' }
                ]
            },
            {
                text: 'Learning Resources',
                items: [
                    { text: '💡 Best Practices', link: '/guide/best-practices' },
                    { text: '🚀 Performance', link: '/guide/performance' },
                    { text: '🏗️ Orchestration', link: '/guide/orchestration' }
                ]
            }

        ],

        socialLinks: [
            { icon: 'github', link: 'https://github.com/Digvijay/gUnit' }
        ],

        footer: {
            message: 'Released under the MIT License.',
            copyright: 'Copyright © 2026 Digvijay'
        }
    }
})
