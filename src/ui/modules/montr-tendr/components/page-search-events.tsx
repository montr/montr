import * as React from "react";
import { Link } from "react-router-dom";
import { Form, Select, Button, DatePicker } from "antd";
import { EventService, EventTemplateService } from "../services";
import { Page, DataTable, PageHeader, DataBreadcrumb, Toolbar, ButtonCreate, Icon } from "@montr-core/components";
import { IEvent } from "../models";

interface Props {
}

interface State {
	configCodes: IEvent[];
}

export default class SearchEvents extends React.Component<Props, State> {

	private readonly eventService = new EventService();
	private readonly eventTemplateService = new EventTemplateService();

	constructor(props: Props) {
		super(props);

		this.state = {
			configCodes: []
		};
	}

	componentDidMount = async (): Promise<void> => {
		await this.fetchConfigCodes();
	};

	componentWillUnmount = async (): Promise<void> => {
		await this.eventTemplateService.abort();
		await this.eventService.abort();
	};

	fetchConfigCodes = async (): Promise<void> => {
		const templates = await this.eventTemplateService.list();

		this.setState({ configCodes: templates?.rows });
	};

	render = (): React.ReactNode => {
		const { configCodes } = this.state;

		return (
			<Page title={<>
				<Toolbar float="right">
					<Link to="/events/new"><ButtonCreate /></Link>
				</Toolbar>

				<DataBreadcrumb items={[]} />
				<PageHeader>Процедуры</PageHeader>
			</>}>

				<Form layout="inline">
					<Form.Item>
						<Select mode="multiple" placeholder="Выберите тип" style={{ minWidth: 200 }}>
							{configCodes && configCodes.map(x => {
								return <Select.Option key={`${x.uid}`} value={`${x.uid}`}>{x.name}</Select.Option>;
							})}
						</Select>
					</Form.Item>
					<Form.Item>
						<DatePicker />
					</Form.Item>
					<Form.Item>
						<Button type="primary" icon={Icon.Search}>Искать</Button>
					</Form.Item>
				</Form>

				<br />

				<DataTable
					rowKey="uid"
					viewId="PrivateEventSearch/Grid"
					loadUrl={this.eventService.getLoadUrl()} />

			</Page>
		);
	};
}
