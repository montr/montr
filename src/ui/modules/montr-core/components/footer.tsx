import * as React from "react";
import { LanguageSelector } from "./";

export const Footer = () => {
	return (<>
		<span>Powered by <a href="https://montr.net" target="_blank">Montr</a></span>

		{/* <a href="https://github.com/montr/montr" style={{ float: "right" }} target="_blank">
			<img alt="GitHub Actions status" src="//github.com/montr/montr/workflows/build/badge.svg" />
		</a> */}

		<span className="language-selector">
			<LanguageSelector />
		</span>
	</>);
};
