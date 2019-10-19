const path = require("path");
const tsImportPluginFactory = require("ts-import-plugin")

module.exports = {
	mode: process.env.test,
	entry: {
		"app": "./modules/host/app.tsx",
	},
	output: {
		path: __dirname,
		filename: (chunkData) => {
			return "../Host/wwwroot/assets/[name].js";

			/* if (chunkData.chunk.name.startsWith("tendr"))
				return "../Host/wwwroot/assets/[name].js";

			if (chunkData.chunk.name === "kompany")
				return "../Kompany.Web/wwwroot/assets/[name].js";

			return null; */
		}
	},
	resolve: {
		alias: {
			"@montr-core": path.resolve(__dirname, "modules/montr-core/"),
			"@montr-master-data": path.resolve(__dirname, "modules/montr-master-data/"),
			"@montr-kompany": path.resolve(__dirname, "modules/montr-kompany/"),
			"@montr-tendr": path.resolve(__dirname, "modules/montr-tendr/"),
		},
		extensions: [".ts", ".tsx", ".js", ".json"]
	},
	module: {
		rules: [
			{
				test: /\.(jsx|tsx|js|ts)$/,
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
					/* compilerOptions: {
						module: "es2015"
					} */
				},
				exclude: /node_modules/
			},

			// All output ".js" files will have any sourcemaps re-processed by "source-map-loader".
			{
				test: /\.js$/, loader: "source-map-loader",
				enforce: "pre", exclude: [/mutationobserver-shim/g]
			},

			{
				test: /\.less$/, use: [
					"style-loader",
					"css-loader",
					{
						loader: "less-loader",
						options: {
							javascriptEnabled: true,
							modifyVars: {
								// https://github.com/ant-design/ant-design/blob/master/components/style/themes/default.less
								// "primary-color": "#1DA57A",
								// "primary-color": "#357ae8", // (?)
								// "link-color": "#1DA57A",
								// "border-radius-base": "4px",
								// "font-size-base": "13px",
								// "font-family": "'Open Sans', 'Helvetica Neue', Helvetica, Arial, sans-serif"
							},
						}
					}
				]
			},
		],
	}
};
