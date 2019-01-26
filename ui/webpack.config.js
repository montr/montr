const path = require("path");
const webpack = require("webpack");
const tsImportPluginFactory = require("ts-import-plugin")

const Globals = {
	"KOMPANY_API_URL": JSON.stringify("http://kompany.montr.io:5010/api")
};

module.exports = {
	// mode: "production",
	mode: "development",
	entry: {
		"tendr": "./modules/tendr/public.tsx",
		"tendr.app": "./modules/tendr/app.tsx",
		"kompany": "./modules/kompany/public.tsx"
	},
	output: {
		path: __dirname,
		filename: (chunkData) => {
			if (chunkData.chunk.name.startsWith("tendr"))
				return "../Tendr/wwwroot/assets/[name].js";

			if (chunkData.chunk.name === "kompany")
				return "../Kompany.Web/wwwroot/assets/[name].js";

			return null;
		}
	},

	// Enable sourcemaps for debugging webpack"s output.
	devtool: "source-map",

	resolve: {
		alias: {
			"@montr-core": path.resolve(__dirname, "modules/montr-core/"),
			"@kompany": path.resolve(__dirname, "modules/kompany/"),
		},
		extensions: [".ts", ".tsx", ".js", ".json"]
	},

	plugins: [
		new webpack.DefinePlugin({
			...Globals
		})
	],

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

			// { test: /\.css$/, use: [ "style-loader", "css-loader" ] },

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
								"primary-color": "#357ae8",
								// "link-color": "#1DA57A",
								// "border-radius-base": "4px",
								// "font-size-base": "13px",
							},
						}
					}
				]
			},
		],
	},

	// When importing a module whose path matches one of the following, just
	// assume a corresponding global variable exists and use that instead.
	// This is important because it allows us to avoid bundling all of our
	// dependencies, which allows browsers to cache those libraries between builds.
	externals: {
		// "react": "React",
		// "react-dom": "ReactDOM"
	}
};
