import * as React from "react";
import { Link } from "react-router-dom";
import { Form, Select, Button, DatePicker } from "antd";
import { EventAPI } from "../../api";
import { DataTable } from "../../components";
import { Page } from "@montr-core/components";

interface Props {
}

interface State {
}

export class SearchEvents extends React.Component<Props, State> {
	constructor(props: Props) {
		super(props);
	}

	render() {
		return (
			<Page title="Торговые процедуры">

				<div style={{ float: "right" }}>
					<Link to="/events/new"><Button icon="plus">Создать</Button></Link>
				</div>

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
					loadUrl={EventAPI.getLoadUrl()} />

			</Page>
		);
	}
}
