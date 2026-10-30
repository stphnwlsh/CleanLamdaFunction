// Copies the logos, icons, link preview and colours this site uses from a checkout of the brand repository.
// Usage: npm run sync-brand -- ../../brand
import fs from 'node:fs';
import path from 'node:path';

const brand = path.resolve(process.argv[2] ?? '../../brand');
const site = path.resolve(import.meta.dirname, '..');
const family = ['minimalapi', 'webapi', 'graphql', 'mcp', 'lambda'];

if (!fs.existsSync(path.join(brand, 'scripts', 'brand.mjs'))) {
  console.error(`${brand} doesn't look like a checkout of LeanMeanCleanApps/brand.`);
  process.exit(1);
}

const copy = (from, to) => {
  fs.mkdirSync(path.dirname(path.join(site, to)), { recursive: true });
  fs.copyFileSync(path.join(brand, from), path.join(site, to));
};

for (const file of ['favicon.svg', 'favicon.ico', 'apple-touch-icon.png']) {
  copy(`assets/logos/lambda/web/${file}`, `public/${file}`);
}
copy('assets/social/png/CleanLamdaFunction.png', 'public/social.png');
copy('assets/logos/lambda/svg/lambda-small.svg', 'src/assets/logo.svg');
copy('assets/logos/lambda/svg/lambda-small-light.svg', 'src/assets/logo-light.svg');
for (const key of family) {
  copy(`assets/logos/${key}/svg/${key}.svg`, `src/assets/logos/${key}.svg`);
  copy(`assets/logos/${key}/svg/${key}-light.svg`, `src/assets/logos/${key}-light.svg`);
}
copy('assets/logos/org-spectrum/svg/org-spectrum-small-tile.svg', 'src/assets/logos/org-spectrum-small-tile.svg');
copy('tokens/tokens.css', 'src/styles/brand/tokens.css');
copy('tokens/starlight/base.css', 'src/styles/brand/base.css');
copy('tokens/starlight/lambda.css', 'src/styles/brand/lambda.css');

console.log(`Copied the brand files from ${brand}.`);
