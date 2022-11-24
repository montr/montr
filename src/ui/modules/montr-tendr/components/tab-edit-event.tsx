import { DataForm } from "@montr-core/components";
import { ApiResult, DataPaneProps, IDataField } from "@montr-core/models";
import { FormInstance } from "antd/es/form";
import * as React from "react";
import { IEvent } from "../models";
import { EventMetadataService, EventService } from "../services";

interface Props {
	data: IEvent;
	formRef?: React.RefObject<FormInstance>;
}

interface State {
	loading: boolean;
	fields?: IDataField[];
}

class EventForm extends React.Component<Props, State> {

	private readonly metadataService = new EventMetadataService();
	private readonly eventService = new EventService();

	constructor(props: Props) {
		super(props);

		this.state = {
			loading: true
		};
	}

	componentDidMount = async (): Promise<void> => {
		await this.fetchData();
	};

	componentWillUnmount = async (): Promise<void> => {
		await this.metadataService.abort();
		await this.eventService.abort();
	};

	fetchData = async (): Promise<void> => {
		const dataView = await this.metadataService.load(`Event/Edit`);

		this.setState({ loading: false, fields: dataView.fields });
	};

	save = async (values: IEvent): Promise<ApiResult> => {
		return await this.eventService.update({ uid: this.props.data.uid, ...values });
	};

	render = (): React.ReactNode => {

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

interface EditEventPaneProps extends DataPaneProps<IEvent> {
	data: IEvent;
}

// todo: remove
export default class TabEditEvent extends React.Component<EditEventPaneProps> {

	private readonly formRef = React.createRef<FormInstance>();

	save(): void {
		const form = this.formRef.current;
		if (form) {
			form.submit();
		}
	}

	render(): React.ReactNode {
		return (
			<EventForm data={this.props.data} formRef={this.formRef} />
		);
	}
}
