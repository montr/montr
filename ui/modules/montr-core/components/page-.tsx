import * as React from "react";

import { PageHeader } from ".";
import { Row, Col } from "antd";

interface PageProps {
	title?: string;
	toolbar?: React.ReactNode;
}

export class Page extends React.Component<PageProps> {
	render() {
		return (
			<div>

				<Row>
					<Col span={12}>
						{this.props.title && <PageHeader>{this.props.title}</PageHeader>}
					</Col>
					<Col span={12} style={{ textAlign: "right" }}>
						{this.props.toolbar}
					</Col>
				</Row>

				{this.props.children}
			</div>
		);
	}
};
