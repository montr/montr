import * as React from "react";
import { Icon } from "antd";

export class Logo extends React.Component {
	public render() {
		return (
			<div className="logo">
				<a href="/">
					<Icon type="html5" theme="twoTone" /> <span>&lt;montr /&gt;</span>
				</a>
			</div>
		);
	}
};
