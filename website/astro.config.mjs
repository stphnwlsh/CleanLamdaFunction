// @ts-check
import { defineConfig } from 'astro/config';
import starlight from '@astrojs/starlight';
import starlightLlmsTxt from 'starlight-llms-txt';

const site = 'https://stphnwlsh.github.io';
const base = '/CleanLamdaFunction';
const repo = 'https://github.com/stphnwlsh/CleanLamdaFunction';

export default defineConfig({
  site,
  base,
  trailingSlash: 'always',
  integrations: [
    starlight({
      title: 'Clean Lambda Function',
      description: 'A .NET 10 dotnet new template for AWS Lambda, built with Clean Architecture minus the ceremony.',
      logo: { src: './src/assets/logo.svg', alt: 'Clean Lambda Function' },
      favicon: '/favicon.svg',
      social: [
        { icon: 'github', label: 'GitHub', href: repo },
        { icon: 'heart', label: 'Sponsor', href: 'https://github.com/sponsors/stphnwlsh' },
      ],
      editLink: { baseUrl: `${repo}/edit/main/website/` },
      lastUpdated: true,
      credits: false,
      customCss: [
        '@fontsource-variable/bricolage-grotesque',
        '@fontsource-variable/inter',
        '@fontsource-variable/jetbrains-mono',
        './src/styles/theme.css',
      ],
      expressiveCode: {
        themes: ['github-dark-default', 'github-light-default'],
        styleOverrides: { borderRadius: '0.75rem', codeFontFamily: "'JetBrains Mono Variable', ui-monospace, monospace" },
      },
      head: [
        { tag: 'link', attrs: { rel: 'apple-touch-icon', href: `${base}/apple-touch-icon.png` } },
        { tag: 'meta', attrs: { property: 'og:image', content: `${site}${base}/social.png` } },
        { tag: 'meta', attrs: { name: 'twitter:card', content: 'summary_large_image' } },
        { tag: 'meta', attrs: { name: 'twitter:image', content: `${site}${base}/social.png` } },
      ],
      components: {
        Footer: './src/components/Footer.astro',
      },
      sidebar: [
        { label: 'Start here', items: ['getting-started', 'guides/run-locally'] },
        {
          label: 'Guides',
          items: ['guides/add-a-function', 'guides/database', 'guides/deploy-to-aws', 'guides/secure-the-function'],
        },
        { label: 'Reference', items: [{ autogenerate: { directory: 'reference' } }] },
        { label: 'Under the hood', items: ['explanation/architecture', 'explanation/decisions', 'explanation/how-its-built'] },
        { label: 'The family', link: '/family/' },
      ],
      plugins: [
        starlightLlmsTxt({
          projectName: 'Clean Lambda Function',
          description: 'A dotnet new template for an AWS Lambda function packaged as a container image, built with a streamlined Clean Architecture on .NET 10.',
        }),
      ],
    }),
  ],
});
