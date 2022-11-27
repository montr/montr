const path = require("path");
const webpack = require('webpack');

const options = {
	// mode: process.env.test,
	entry: {
		"app": "./modules/host/app.tsx",
	},
	output: {
		path: path.resolve(__dirname, process.env.CI ? "./assets" : "../Host/wwwroot/assets"),
		filename: "[name].bundle.js",
		chunkFilename: "[name].chunk.js",
		publicPath: "/assets/",
		clean: true
	},
	resolve: {
		alias: {
			"@montr-automate": path.resolve(__dirname, "modules/montr-automate/"),
			"@montr-core": path.resolve(__dirname, "modules/montr-core/"),
			"@montr-docs": path.resolve(__dirname, "modules/montr-docs/"),
			"@montr-idx": path.resolve(__dirname, "modules/montr-idx/"),
			"@montr-kompany": path.resolve(__dirname, "modules/montr-kompany/"),
			"@montr-kompany-registration": path.resolve(__dirname, "modules/montr-kompany-registration/"),
			"@montr-master-data": path.resolve(__dirname, "modules/montr-master-data/"),
			"@montr-settings": path.resolve(__dirname, "modules/montr-settings/"),
			"@montr-tasks": path.resolve(__dirname, "modules/montr-tasks/"),
			"@montr-tendr": path.resolve(__dirname, "modules/montr-tendr/"),
		},
		extensions: [".ts", ".tsx", ".js", ".json"]
	},
	module: {
		rules: [
			{
				test: /\.(jsx|tsx|js|ts)$/,
				loader: "babel-loader",
				exclude: /node_modules/
			}, {
				test: /\.js$/,
				loader: "source-map-loader",
				enforce: "pre",
				exclude: [/mutationobserver-shim/g]
			},
			{
				test: /\.(css|less)$/,
				use: [
					{
						loader: "style-loader" // creates style nodes from JS strings
					},
					{
						loader: "css-loader", // translates CSS into CommonJS
						options: {
							sourceMap: false,
						}
					},
					{
						loader: "less-loader", // compiles Less to CSS
						options: {
							sourceMap: false,
							lessOptions: {
								javascriptEnabled: true
							}
						}
					}
				]
			}
		]
	},
	plugins: [
		/* new webpack.optimize.LimitChunkCountPlugin({
			maxChunks: 11
		}), */
		new (require("fork-ts-checker-webpack-plugin"))({
			typescript: {
				diagnosticOptions: {
					semantic: true,
					syntactic: true,
					declaration: true,
					global: true
				}
			},
			issue: {
				severity: "warning"
			}
		})
	]
};

module.exports = options;
