import sharp from 'sharp';
import { readFileSync } from 'fs';
import { fileURLToPath } from 'url';
import { dirname, join } from 'path';

const __filename = fileURLToPath(import.meta.url);
const __dirname = dirname(__filename);

const sizes = [72, 96, 128, 144, 152, 192, 384, 512];
const svgPath = join(__dirname, '../static/icons/icon.svg');
const outputDir = join(__dirname, '../static/icons');

async function generateIcons() {
	const svgBuffer = readFileSync(svgPath);

	console.log('Generating PWA icons...');

	for (const size of sizes) {
		try {
			await sharp(svgBuffer)
				.resize(size, size)
				.png()
				.toFile(join(outputDir, `icon-${size}x${size}.png`));
			console.log(`✓ Generated icon-${size}x${size}.png`);
		} catch (error) {
			console.error(`✗ Failed to generate icon-${size}x${size}.png:`, error.message);
		}
	}

	// Generate apple-touch-icon
	try {
		await sharp(svgBuffer)
			.resize(180, 180)
			.png()
			.toFile(join(outputDir, 'apple-touch-icon.png'));
		console.log('✓ Generated apple-touch-icon.png');
	} catch (error) {
		console.error('✗ Failed to generate apple-touch-icon.png:', error.message);
	}

	// Generate favicon
	try {
		await sharp(svgBuffer)
			.resize(32, 32)
			.png()
			.toFile(join(outputDir, 'favicon-32x32.png'));
		console.log('✓ Generated favicon-32x32.png');

		await sharp(svgBuffer)
			.resize(16, 16)
			.png()
			.toFile(join(outputDir, 'favicon-16x16.png'));
		console.log('✓ Generated favicon-16x16.png');
	} catch (error) {
		console.error('✗ Failed to generate favicons:', error.message);
	}

	console.log('\nAll icons generated successfully!');
}

generateIcons().catch(console.error);
