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
                    { text: '🛣️ Roadmap (v0.4.0)', link: '/roadmap' }
                ]
            },
            {
                text: 'Core Concepts',
                items: [
                    { text: '🛡️ Bounded Parallelism', link: '/concepts/parallelism' },
                    { text: '💉 Dependency Injection', link: '/concepts/di' },
                    { text: '🧩 Hybrid MTP', link: '/concepts/mtp' }
                ]
            },
            {
                text: 'API Reference',
                items: [
                    { text: '🏷️ Attributes', link: '/api/attributes' },
                    { text: '✅ Assertions', link: '/api/assertions' },
                    { text: '⚙️ Configuration', link: '/api/config' }
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
