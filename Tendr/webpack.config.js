const tsImportPluginFactory = require("ts-import-plugin")

module.exports = {
	// mode: "production",
	mode: "development",
	entry: {
		public: "./node_src/public.tsx",
		app: "./node_src/app.tsx",
	},
	output: {
		filename: "[name].js",
		path: __dirname + "/wwwroot/assets"
	},

	// Enable sourcemaps for debugging webpack"s output.
	devtool: "source-map",

	resolve: {
		// Add ".ts" and ".tsx" as resolvable extensions.
		extensions: [".ts", ".tsx", ".js", ".json"]
	},

	module: {
		rules: [
			// All files with a ".ts" or ".tsx" extension will be handled by "awesome-typescript-loader".
			{
				test: /\.(jsx|tsx|js|ts)$/,
				loader: "awesome-typescript-loader",
				options: {
					transpileOnly: true,
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
			{ enforce: "pre", test: /\.js$/, loader: "source-map-loader" },

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
								// "link-color": "#1DA57A",
								// "border-radius-base": "4px",
							},
						}
					}
				]
			},
		]
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