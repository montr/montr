import * as React from "react";

interface Props {
	children: React.ReactNode;
}

export class PageHeader extends React.Component<Props> {
	public render(): React.ReactNode {
		return (
			<h2>{this.props.children}</h2>
		);
	}
}
