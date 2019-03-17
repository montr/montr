import * as React from "react";

export class PageHeader extends React.Component {
	public render() {
		return (
			<h2>{this.props.children}</h2>
		);
	}
};
