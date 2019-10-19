import * as React from "react";
import { Form, Spin } from "antd";
import { FormComponentProps } from "antd/lib/form";
import { IApiResult, IPaneProps, IFormField } from "@montr-core/models";
import { EventService, EventMetadataService } from "../services";
import { IPaneComponent, DataForm } from "@montr-core/components";
import { IEvent } from "../models";

interface IProps extends FormComponentProps {
	data: IEvent;
}

interface IState {
	loading: boolean;
	fields?: IFormField[];
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
	}

	componentWillUnmount = async () => {
		await this._metadataService.abort();
		await this._eventService.abort();
	}

	fetchData = async () => {
		const dataView = await this._metadataService.load(`Event/Edit`);

		this.setState({ loading: false, fields: dataView.fields });
	}

	save = async (values: IEvent): Promise<IApiResult> => {
		return await this._eventService.update({ uid: this.props.data.uid, ...values });
	}

	render = () => {

		const { data } = this.props,
			{ loading, fields } = this.state;

		return (
			<DataForm fields={fields} data={data} onSave={this.save} />
		);
	}
}

const WrappedForm = Form.create<IProps>()(EventForm);

interface IEditEventPaneProps extends IPaneProps<IEvent> {
	data: IEvent;
}

interface IEditEventTabState {
}

export class TabEditEvent extends React.Component<IEditEventPaneProps, IEditEventTabState> {

	private _formRef: IPaneComponent;

	save() {
		this._formRef.save();
	}

	render() {
		return (
			<WrappedForm data={this.props.data} wrappedComponentRef={(form: any) => this._formRef = form} />
		);
	}
}
