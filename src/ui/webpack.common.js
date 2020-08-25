const path = require("path");
const tsImportPluginFactory = require("ts-import-plugin");
const ForkTsCheckerWebpackPlugin = require("fork-ts-checker-webpack-plugin");
const copyPlugin = require("copy-webpack-plugin");

const options = {
	// mode: process.env.test,
	entry: {
		"app": "./modules/host/app.tsx",
	},
	output: {
		path: path.resolve(__dirname, "assets"),
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
			"@montr-tendr": path.resolve(__dirname, "modules/montr-tendr/"),
		},
		extensions: [".ts", ".tsx", ".js", ".json"]
	},
	module: {
		rules: [
			{
				test: /\.(jsx|tsx|js|ts)$/,
				loader: "babel-loader",
				/* loader: "ts-loader",
				loader: "awesome-typescript-loader",
				options: {
					transpileOnly: false,
					getCustomTransformers: () => ({
						before: [tsImportPluginFactory({
							libraryName: "antd",
							libraryDirectory: "lib",
							style: true
						})]
					}),
				}, */
				exclude: /node_modules/
			},

			// All output ".js" files will have any sourcemaps re-processed by "source-map-loader".
			{
				test: /\.js$/,
				loader: "source-map-loader",
				enforce: "pre", exclude: [/mutationobserver-shim/g]
			},

			{
				test: /\.less$/, use: [
					"style-loader",
					"css-loader",
					{
						loader: "less-loader",
						options: {
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
									"font-family": "Nunito, 'Helvetica Neue', Helvetica, Arial, sans-serif"
								}
							}
						}
					}
				]
			},
		],
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
