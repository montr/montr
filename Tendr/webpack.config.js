module.exports = {
	mode: "development",
	entry: "./node_src/index.tsx",
	output: {
		filename: "bundle.js",
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
			{ test: /\.tsx?$/, loader: "awesome-typescript-loader" },

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