import * as React from "react";
import { LanguageSelector } from "./";

export const Footer = () => {
	return (<>
		<span>Powered by <a href="https://montr.net">Montr</a></span>

		<a href="https://github.com/montr/montr/actions" style={{ float: "right" }} target="_blank">
			<img alt="GitHub Actions status" src="//github.com/montr/montr/workflows/build/badge.svg" />
		</a>

		<span className="language-selector">
			<LanguageSelector />
		</span>
	</>);
}
