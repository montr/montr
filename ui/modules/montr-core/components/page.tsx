import * as React from "react";
import { PageHeader } from ".";
import { Row, Col, Affix } from "antd";

interface PageProps {
	title?: string | React.ReactNode;
	toolbar?: React.ReactNode;
}

export class Page extends React.Component<PageProps> {
	render() {

		const { title, toolbar, children } = this.props;

		return (
			<div>

				<Affix>
					<Row>
						<Col span={16}>
							{(typeof title === "string") ? <PageHeader>{title}</PageHeader> : title}
						</Col>
						<Col span={8} style={{ textAlign: "right" }}>
							{toolbar}
						</Col>
					</Row>
				</Affix>

				{children}

			</div>
		);
	}
};
