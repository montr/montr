import * as React from "react";
import { PageHeader } from ".";

interface Props {
	title?: string | React.ReactNode;
	children: React.ReactNode;
}

export class Page extends React.Component<Props> {
	render = (): React.ReactNode => {

		const { title, children } = this.props;

		return (
			<div>
				{/* <Affix> */}
				{(typeof title === "string") ? <PageHeader>{title}</PageHeader> : title}
				{/* </Affix> */}
				{children}
			</div>
		);
	};
}
