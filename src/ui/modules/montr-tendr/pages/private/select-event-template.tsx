import * as React from "react";
import { EventService } from "../../services";
import { Redirect } from "react-router-dom";
import { Constants } from "@montr-core/.";
import { Page, DataTable } from "@montr-core/components";
import { IEvent } from "../../models";
import { IApiResult, Guid, IMenu } from "@montr-core/models";
import { RouteBuilder } from "../../module";

interface IProps {
}

interface IState {
	newUid?: Guid;
}

export default class SelectEventTemplate extends React.Component<IProps, IState> {

	private _eventService = new EventService();

	constructor(props: IProps) {
		super(props);

		this.state = {
		};
	}

	componentWillUnmount = async () => {
		await this._eventService.abort();
	}

	handleSelect = async (data: IEvent) => {
		const result: IApiResult = await this._eventService.insert({
			templateUid: data.uid,
			configCode: data.configCode
		});

		this.setState({ newUid: result.uid });
	}

	render = () => {
		const { newUid } = this.state;

		if (newUid) {
			return <Redirect to={RouteBuilder.editEvent(newUid.toString())} />
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
	}
}
