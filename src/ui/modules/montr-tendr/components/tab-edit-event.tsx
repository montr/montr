import * as React from "react";
import { ApiResult, PaneProps, IDataField } from "@montr-core/models";
import { EventService, EventMetadataService } from "../services";
import { DataForm } from "@montr-core/components";
import { IEvent } from "../models";
import { FormInstance } from "antd/lib/form";

interface IProps {
	data: IEvent;
	formRef?: React.RefObject<FormInstance>;
}

interface IState {
	loading: boolean;
	fields?: IDataField[];
}

class EventForm extends React.Component<IProps, IState> {
	_metadataService = new EventMetadataService();
	_eventService = new EventService();

	constructor(props: IProps) {
		super(props);

		this.state = {
			loading: true
		};
	}

	componentDidMount = async () => {
		await this.fetchData();
	};

	componentWillUnmount = async () => {
		await this._metadataService.abort();
		await this._eventService.abort();
	};

	fetchData = async () => {
		const dataView = await this._metadataService.load(`Event/Edit`);

		this.setState({ loading: false, fields: dataView.fields });
	};

	save = async (values: IEvent): Promise<ApiResult> => {
		return await this._eventService.update({ uid: this.props.data.uid, ...values });
	};

	render = () => {

		const { data, formRef } = this.props,
			{ loading, fields } = this.state;

		return (
			<DataForm
				formRef={formRef}
				fields={fields}
				data={data}
				onSubmit={this.save} />
		);
	};
}

interface IEditEventPaneProps extends PaneProps<IEvent> {
	data: IEvent;
}

interface IEditEventTabState {
}

// todo: remove
export class TabEditEvent extends React.Component<IEditEventPaneProps, IEditEventTabState> {

	private _formRef = React.createRef<FormInstance>();

	save() {
		this._formRef.current.submit();
	}

	render() {
		return (
			<EventForm data={this.props.data} formRef={this._formRef} />
		);
	}
}
