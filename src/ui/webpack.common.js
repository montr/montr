const path = require("path");
const ForkTsCheckerWebpackPlugin = require("fork-ts-checker-webpack-plugin");
const copyPlugin = require("copy-webpack-plugin");

const options = {
	// mode: process.env.test,
	entry: {
		"app": "./modules/host/app.tsx",
	},
	output: {
		path: path.resolve(__dirname, "./assets"),
		filename: "[name].bundle.js",
		chunkFilename: "[name].chunk.js",
		publicPath: "/assets/"
	},
	resolve: {
		alias: {
			"@montr-core": path.resolve(__dirname, "modules/montr-core/"),
			"@montr-idx": path.resolve(__dirname, "modules/montr-idx/"),
			"@montr-master-data": path.resolve(__dirname, "modules/montr-master-data/"),
			"@montr-automate": path.resolve(__dirname, "modules/montr-automate/"),
			"@montr-docs": path.resolve(__dirname, "modules/montr-docs/"),
			"@montr-kompany": path.resolve(__dirname, "modules/montr-kompany/"),
			"@montr-kompany-registration": path.resolve(__dirname, "modules/montr-kompany-registration/"),
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
				test: /\.less$/,
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
								javascriptEnabled: true,
								modifyVars: {
									// https://github.com/ant-design/ant-design/blob/master/components/style/themes/default.less
									// "primary-color": "#1DA57A",
									// "primary-color": "#357ae8", // (?)
									// "link-color": "#1DA57A",
									// "border-radius-base": "4px",
									// "font-size-base": "13px",
									// "font-family": "'Open Sans', 'Helvetica Neue', Helvetica, Arial, sans-serif"
									// "font-family": "'Segoe UI', 'Helvetica Neue', Helvetica, Arial, sans-serif"
									// "font-family": "Nunito, 'Helvetica Neue', Helvetica, Arial, sans-serif"
								}
							}
						}
					}
				]
			}
		]
	},
	plugins: [
		new ForkTsCheckerWebpackPlugin()
	]
};

if (!process.env.CI) {
	options.plugins.push(
		new copyPlugin({
			patterns: [
				{ from: "./assets/", to: "../../Host/wwwroot/assets" }
			]
		})
	);
}

module.exports = options;
