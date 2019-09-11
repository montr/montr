import * as React from "react";
import { Icon } from "antd";

export const Footer = () => {
	return (<>
		© {new Date().getFullYear()} Montr

		<a href="https://github.com/montr/montr" style={{ color: "gray", float: "right" }} target="_blank">
			<Icon type="github" />
		</a>
	</>);
}
