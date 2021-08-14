import { IMenu } from "@montr-core/models";
import { Card, List, Skeleton } from "antd";
import * as React from "react";
import { Page } from ".";

export default class PageDashboard extends React.Component {

	render(): React.ReactNode {
		const data: IMenu[] = [];

		data.push({ name: `Common Actions (Create/Manage)` });
		data.push({ name: `Recently Viewed` });
		data.push({ name: `Calendar with To-Do items` });
		data.push({ name: `My Documents` });
		data.push({ name: `Events stats (year/month/select period)` });

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
