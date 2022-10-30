import { DataForm } from "@montr-core/components";
import { ApiResult, Guid, IDataField } from "@montr-core/models";
import { DataFormChanges } from "@montr-core/models/data-form-changes";
import { MetadataService } from "@montr-core/services";
import { Spin } from "antd";
import { FormInstance } from "antd/lib/form";
import { FieldData } from "rc-field-form/lib/interface";
import React from "react";
import { Automation } from "../models";
import { Views } from "../module";
import { AutomationService } from "../services";
import { AutomationContextProvider } from "./automation-context";

interface Props {
	entityTypeCode: string;
	entityUid: Guid;
	data?: Automation;
}

interface State {
	loading: boolean;
	fields?: IDataField[];
	dataFormChanges?: DataFormChanges;
}

// rename AutomationContextProvider to ClassifierContextProvider and use in classifier edit page
export default class PaneEditAutomation extends React.Component<Props, State> {

	private readonly metadataService = new MetadataService();
	private readonly automationService = new AutomationService();
	private readonly formRef = React.createRef<FormInstance>();

	constructor(props: Props) {
		super(props);

		this.state = {
			loading: true,
		};
	}

	componentDidMount = async (): Promise<void> => {
		await this.fetchData();
	};

	componentWillUnmount = async (): Promise<void> => {
		await this.metadataService.abort();
		await this.automationService.abort();
	};

	fetchData = async (): Promise<void> => {

		const dataView = await this.metadataService.load(Views.automationForm);

		this.setState({
			loading: false,
			fields: dataView?.fields || [],
		});
	};

	handleSubmitClick = async (e: React.MouseEvent<any>): Promise<void> => {
		await this.formRef.current.submit();
	};

	handleFieldsChange = async (changedFields: FieldData[], allFields: FieldData[]): Promise<void> => {
		this.setState({
			dataFormChanges: { changedFields, allFields }
		});
	};

	handleValuesChange = async (changedValues: Automation, values: Automation): Promise<void> => {
		this.setState({
			dataFormChanges: { values, changedValues }
		});
	};

	handleSubmit = async (values: Automation): Promise<ApiResult> => {
		const { entityUid } = this.props;

		const result = await this.automationService.updateRules({
			automationUid: entityUid,
			conditions: values.conditions,
			actions: values.actions
		});

		return result;
	};

	render = (): React.ReactNode => {
		const { data } = this.props,
			{ loading, fields, dataFormChanges } = this.state;

		return (<>
			<Spin spinning={loading}>

				<AutomationContextProvider data={data} dataFormChanges={dataFormChanges}>
					<DataForm
						formRef={this.formRef}
						fields={fields}
						data={data}
						onFieldsChange={this.handleFieldsChange}
						// onValuesChange={this.handleValuesChange}
						onSubmit={this.handleSubmit}
					/>
				</AutomationContextProvider>

			</Spin>
		</>);
	};
}
