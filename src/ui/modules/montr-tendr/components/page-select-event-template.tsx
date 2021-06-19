import * as React from "react";
import { EventService } from "../services";
import { Redirect } from "react-router-dom";
import { Constants } from "@montr-core/.";
import { Page, DataTable } from "@montr-core/components";
import { IEvent } from "../models";
import { ApiResult, Guid, IMenu } from "@montr-core/models";
import { RouteBuilder } from "../module";

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
					loadUrl={`${Constants.apiURL}/EventTemplate/List/`}
					rowActions={rowActions}
				/>

			</Page>
		);
	};
}
