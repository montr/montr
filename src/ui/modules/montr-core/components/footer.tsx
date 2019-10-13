import * as React from "react";
import { LanguageSelector } from "./";

export const Footer = () => {
	return (<>
		© {new Date().getFullYear()} Montr

		<a href="https://github.com/montr/montr/actions" style={{ float: "right" }} target="_blank">
			<img alt="GitHub Actions status" src="//github.com/montr/montr/workflows/build/badge.svg" />
		</a>

		<span style={{ float: "right", paddingRight: 10 }}>
			<LanguageSelector />
		</span>
	</>);
}
