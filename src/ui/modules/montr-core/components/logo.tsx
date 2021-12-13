import * as React from "react";
import { Icon } from "./";

export class Logo extends React.Component {
	render = (): React.ReactNode => {
		return (
			<div className="logo">
				<a href="/">
					{Icon.Html5TwoTone} <span>&lt;montr /&gt;</span>
				</a>
			</div>
		);
	};
}
