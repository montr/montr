import * as React from "react";
import { Icons } from "./icons";

export class Logo extends React.Component {
	public render() {
		return (
			<div className="logo">
				<a href="/">
					{Icons.get("html5-2t")} <span>&lt;montr /&gt;</span>
				</a>
			</div>
		);
	}
};
