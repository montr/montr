import * as React from "react";
import { LanguageSelector } from "./language-selector";

export const Footer = (): React.ReactElement => {
	return <>
		<span>Powered by <a href="https://github.com/montr/montr" target="_blank">Montr</a></span>

		<a href="https://github.com/montr/montr/actions" style={{ float: "right" }} target="_blank">
			<img alt="GitHub Actions status" src="//github.com/montr/montr/workflows/build/badge.svg" />
		</a>

		<span className="language-selector">
			<LanguageSelector />
		</span>
	</>;
};
