import { DataTable, Page } from "@montr-core/components";
import { ApiResult, Guid, IMenu } from "@montr-core/models";
import * as React from "react";
import { Redirect } from "react-router-dom";
import { IEvent } from "../models";
import { Api, RouteBuilder } from "../module";
import { EventService } from "../services";

interface Props {
}

interface State {
	newUid?: Guid;
}

export default class SelectEventTemplate extends React.Component<Props, State> {

	private readonly eventService = new EventService();

	constructor(props: Props) {
		super(props);

		this.state = {
		};
	}

	componentWillUnmount = async (): Promise<void> => {
		await this.eventService.abort();
	};

	handleSelect = async (data: IEvent): Promise<void> => {
		const result: ApiResult = await this.eventService.insert({
			templateUid: data.uid,
			configCode: data.configCode
		});

		this.setState({ newUid: result.uid });
	};

	render = (): React.ReactNode => {
		const { newUid } = this.state;

		if (newUid) {
			return <Redirect to={RouteBuilder.editEvent(newUid.toString())} />;
		}

		const rowActions: IMenu[] = [
			{ name: "Выбрать", onClick: this.handleSelect }
		];

		return (
			<Page title="Выберите шаблон процедуры">

				<DataTable
					rowKey="uid"
					viewId="PrivateEventSearch/Grid"
					loadUrl={Api.eventTemplateList}
					rowActions={rowActions}
				/>

			</Page>
		);
	};
}
