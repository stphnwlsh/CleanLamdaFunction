# Clean Lambda Function docs

The documentation site at <https://stphnwlsh.github.io/CleanLamdaFunction/>, built with [Starlight](https://starlight.astro.build). The `Docs` workflow builds it on pull requests that touch this folder and deploys it from `main`.

## Run it locally

You need Node 24 (see `.nvmrc`).

```bash
npm ci
npm run dev
```

The pages live in `src/content/docs`. Humour belongs on the home page, the tutorial and the explanations. Reference pages and steps stay dead straight, because nobody wants a joke in the middle of a config table.

## Logos and colours

The logos, icons, link preview and colours come from the [Lean Mean Clean Apps brand](https://github.com/LeanMeanCleanApps/brand). Don't edit the copies in `src/assets`, `public` or `src/styles/brand`. Change the brand repository, then copy them across from a checkout of it:

```bash
npm run sync-brand -- ../../brand
```

`src/styles/theme.css` holds the few extras that are particular to this site.
