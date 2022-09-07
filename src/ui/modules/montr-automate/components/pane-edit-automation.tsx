import { DataForm } from "@montr-core/components";
import { ApiResult, Guid, IDataField } from "@montr-core/models";
import { MetadataService } from "@montr-core/services";
import { Spin } from "antd";
import { FormInstance } from "antd/lib/form";
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
}

// todo: remove (after moving to classifiers),
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
		const { entityTypeCode, entityUid, data } = this.props,
			{ loading, fields } = this.state;

		return (<>
			<Spin spinning={loading}>

				{/* todo: pass Automation to context? */}
				<AutomationContextProvider entityTypeCode={entityTypeCode} entityUid={entityUid}>
					<DataForm
						formRef={this.formRef}
						fields={fields}
						data={data}
						onSubmit={this.handleSubmit} />
				</AutomationContextProvider>

			</Spin>
		</>);
	};
}
