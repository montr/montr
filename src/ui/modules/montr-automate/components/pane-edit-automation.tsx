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
	uid?: Guid;
	onSuccess?: () => void;
	onClose?: () => void;
}

interface State {
	loading: boolean;
	data?: Automation;
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
		const { entityTypeCode, entityUid, uid } = this.props;

		const data: Automation = this.props.data;
		// await this.automationService.get(entityTypeCode, entityUid, uid);

		const dataView = await this.metadataService.load(Views.automationForm);

		this.setState({
			loading: false,
			data,
			fields: dataView?.fields || [],
		});
	};

	handleSubmitClick = async (e: React.MouseEvent<any>): Promise<void> => {
		await this.formRef.current.submit();
	};

	handleSubmit = async (values: Automation): Promise<ApiResult> => {
		const { entityTypeCode, entityUid, uid, onSuccess } = this.props;

		const item = { ...values };

		const result = await this.automationService.updateRules({
			entityTypeCode,
			entityUid,
			item: { uid: entityUid, typeCode: "temp", ...item }
		});

		if (result.success && onSuccess) {
			onSuccess();
		}

		return result;
	};

	render = (): React.ReactNode => {
		const { entityTypeCode, entityUid, onClose } = this.props,
			{ loading, data, fields } = this.state;

		return (<>
			<Spin spinning={loading}>
				{/* <Drawer
					title="Automation"
					closable={true}
					onClose={onClose}
					visible={true}
					width={800}
					footer={
						<Toolbar clear size="small" float="right">
							<ButtonCancel onClick={onClose} />
							<ButtonSave onClick={this.handleSubmitClick} />
						</Toolbar>}> */}

				{/* todo: pass Automation to context? */}
				<AutomationContextProvider entityTypeCode={entityTypeCode} entityUid={entityUid}>
					<DataForm
						formRef={this.formRef}
						fields={fields}
						data={data}
						onSubmit={this.handleSubmit} />
				</AutomationContextProvider>

				{/* </Drawer> */}
			</Spin>
		</>);
	};
}
