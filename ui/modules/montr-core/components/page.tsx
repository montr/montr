import * as React from "react";
import { PageHeader } from ".";
import { Row, Col } from "antd";

interface PageProps {
	title?: string | React.ReactNode;
	toolbar?: React.ReactNode;
}

export class Page extends React.Component<PageProps> {
	render() {

		const { title, toolbar, children } = this.props;

		return (
			<div>

				<Row>
					<Col span={12}>
						{(typeof title === "string") ? <PageHeader>{title}</PageHeader> : title}
					</Col>
					<Col span={12} style={{ textAlign: "right" }}>
						{toolbar}
					</Col>
				</Row>

				{children}

			</div>
		);
	}
};
