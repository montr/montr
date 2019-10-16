const merge = require("webpack-merge");
const common = require("./webpack.common.js");
// const BundleAnalyzerPlugin = require('webpack-bundle-analyzer').BundleAnalyzerPlugin;
// const HardSourceWebpackPlugin = require('hard-source-webpack-plugin');

module.exports = merge(common, {
	mode: "development",
	devtool: "source-map",
	plugins: [
		// new BundleAnalyzerPlugin()
		// new HardSourceWebpackPlugin()
	],
});
