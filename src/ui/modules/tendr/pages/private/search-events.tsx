import * as React from "react";
import { Link } from "react-router-dom";
import { Form, Select, Button, DatePicker } from "antd";
import { EventService } from "../../services";
import { Page, DataTable, PageHeader, DataBreadcrumb, Toolbar } from "@montr-core/components";

interface Props {
}

interface State {
}

export class SearchEvents extends React.Component<Props, State> {

	private _eventService = new EventService();

	constructor(props: Props) {
		super(props);
	}

	componentWillUnmount = async () => {
		await this._eventService.abort();
	}

	render() {
		return (
			<Page title={<>
				<Toolbar float="right">
					<Link to="/events/new"><Button icon="plus">Создать</Button></Link>
				</Toolbar>

				<DataBreadcrumb items={[]} />
				<PageHeader>Торговые процедуры</PageHeader>
			</>}>

				<Form layout="inline">
					<Form.Item>
						<Select mode="multiple" placeholder="Выберите тип" style={{ minWidth: 200 }}>
							<Select.Option value="0">Запрос предложений</Select.Option>
							<Select.Option value="1">Предложение</Select.Option>
						</Select>
					</Form.Item>
					<Form.Item>
						<DatePicker />
					</Form.Item>
					<Form.Item>
						<Button type="primary" icon="search">Искать</Button>
					</Form.Item>
				</Form>

				<br />

				<DataTable
					viewId="PrivateEventSearch/Grid"
					loadUrl={this._eventService.getLoadUrl()} />

			</Page>
		);
	}
}
