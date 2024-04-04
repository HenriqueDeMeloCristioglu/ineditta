const path = require('path');
const glob = require('glob');
const CopyPlugin = require('copy-webpack-plugin');
const MiniCssExtractPlugin = require('mini-css-extract-plugin');
const { ProvidePlugin } = require('webpack');

module.exports = {
	entry: () => {
		const entries = {};
		glob.sync('./src/pages/**/*.js').forEach(file => {
			const parts = file.split('/');
			const lastIndex = parts.length - 1;
			const name = parts[lastIndex].replace('.js', '');

			entries[name] = file;
		});
		glob.sync('./src/js/scripts/**/*.js').forEach(file => {
			const name = file.replace('./src/js/scripts', '').replace('.js', '');
			entries[name] = file;
		})
		glob.sync('./src/demo/**/*.js').forEach(file => {
			const name = file.replace('./src/demo/', 'demo/').replace('.js', '');
			entries[name] = file;
		})
		glob.sync('./src/components/**/*.js').forEach(file => {
			const relativePath = path.relative('./src/components', file);
			const name = path.basename(relativePath, '.js');
			entries[name] = file;
    })
    glob.sync('./src/includes/js/*.js').forEach(file => {
      const name = file.replace('./src/includes/js/', 'temp/').replace('.js', '');
      entries[name] = file;
    })

		entries['core'] = './src/js/core.js';
		entries['main'] = './src/js/main.js';

		return entries;
	},
	output: {
		filename: 'js/[name].min.js',
		path: path.resolve(__dirname, 'dist'),
	},
	module: {
		rules: [
			{
				test: /\.svg$/i,
				type: 'asset/resource',
				loader: 'svgo-loader'
			},
			{
				test: /\.css$/i,
				use: [MiniCssExtractPlugin.loader, "css-loader"],
			},
			{
				test: /\.(woff|woff2|eot|ttf|otf)$/i,
				type: 'asset/resource',
			},
			{
				test: /\.(jpe?g|png|gif|svg)$/i,
				type: "asset",
				include: path.resolve(__dirname, 'src/assets/images'), // Include images path
				use: [
					{
						loader: 'file-loader',
						options: {
							name: 'assets/images/[name].[ext]',
						}
					}]
			},
			{
				test: /\.html$/,
				exclude: /node_modules/,
				use: { loader: 'html-loader' }
			}
		],
	},
	plugins: [
		{
            apply: (compiler) => {
                compiler.hooks.afterCompile.tap('AfterCompilePlugin', (compilation) => {
                    // Specify your PHP files or directories here
                    compilation.contextDependencies.add(path.join(__dirname, '**/*.php'));
                });
            }
        },
		new ProvidePlugin({
			jQuery: 'jquery',
			'window.jQuery': 'jquery',
			$: 'jquery',
		}),
		// Add the CopyPlugin to copy PHP files to the output directory
		new CopyPlugin({
			patterns: [
				{
					from: 'src/pages/**/*.php',
					to: '[name][ext]'
				},
				{
					from: 'src/pages/**/*.html',
					to: '[name][ext]'
				},
				{
					from: 'src/index.php',
					to: 'index.php'
				},
				{
					from: 'src/components/**/*.php',
					to: '[name][ext]'
				},
				{
					from: 'src/assets/configs/config.json',
					to: 'assets/configs/[name].[hash].json',
				},
				{
					from: 'src/assets/configs/keycloak.json',
					to: 'keycloak.json',
				},
				{
					from: 'src/assets/images/**/*.*',
					to: 'assets/images/[name][ext]'
				},
				{
					from: 'src/includes/php',
					to: 'includes/php'
				},
				{
					from: 'src/includes/config',
					to: 'includes/config'
				},
				{
					from: 'src/includes/css',
					to: 'includes/css'
				},
				{
					from: 'src/includes/demo',
					to: 'includes/demo'
				},
				{
					from: 'src/includes/fonts',
					to: 'includes/fonts'
				},
				{
					from: 'src/includes/img',
					to: 'includes/img'
				},
				{
					from: 'src/js/translate',
					to: 'js/translate'
        },
        {
          from: 'src/includes/plugins',
          to: 'js/temp/plugins'
        }
			],
		}),
		new MiniCssExtractPlugin({
			filename: '[name].css',
		}),
		// new webpack.HotModuleReplacementPlugin(),
	],
	resolve: {
		fallback: {
			fs: false,
			os: false,
			path: false,
			url: false
		}
	},
	mode: 'development',
};
