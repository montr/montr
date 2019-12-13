import * as React from "react";
import { Page } from ".";
import { List, Card, Skeleton } from "antd";
import { IMenu } from "@montr-core/models";

export default class Dashboard extends React.Component {

	render() {
		const data: IMenu[] = [];
		for (var i = 0; i < 6; i++) {
			data.push({ name: `Card Title ${i}` });
		}

		return (
			<Page title="Панель управления">
				<List
					grid={{ gutter: 12, column: 3 }}
					dataSource={data}
					renderItem={item => (
						<List.Item>
							<Card size="small" title={item.name} extra={<a>Settings</a>} >
								<Skeleton active />
							</Card>
						</List.Item>
					)}
				/>
			</Page>
		);
	}
}
