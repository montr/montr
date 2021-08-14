import { Button, Result } from "antd";
import * as React from "react";

export default class PageError404 extends React.Component {
	render(): React.ReactNode {
		return (
			<Result
				status={404}
				title={<h2>404</h2>}
				subTitle="Sorry, the page you visited does not exist."
				extra={<Button type="primary">Back Home</Button>}
			/>
		);
	}
}
