import { ButtonCancel, ButtonSave, DataForm, Toolbar } from "@montr-core/components";
import { ApiResult, Guid, IDataField } from "@montr-core/models";
import { MetadataService } from "@montr-core/services";
import { Drawer, Spin } from "antd";
import { FormInstance } from "antd/lib/form";
import React from "react";
import { Automation } from "../models";
import { Views } from "../module";
import { AutomationService } from "../services";
import { AutomationContextProvider } from "./automation-context";

interface Props {
	entityTypeCode: string;
	entityUid: Guid | string;
	uid?: Guid;
	onSuccess?: () => void;
	onClose?: () => void;
}

interface State {
	loading: boolean;
	data?: Automation;
	fields?: IDataField[];
}

export class PaneEditAutomation extends React.Component<Props, State> {

	private _metadataService = new MetadataService();
	private _automationService = new AutomationService();
	private _formRef = React.createRef<FormInstance>();

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
		await this._metadataService.abort();
		await this._automationService.abort();
	};

	fetchData = async (): Promise<void> => {
		const { entityTypeCode, entityUid, uid } = this.props;

		const data: Automation = (uid)
			? await this._automationService.get(entityTypeCode, entityUid, uid)
			// todo: load defaults from server
			: {
				conditions: [],
				actions: []
			};

		const dataView = await this._metadataService.load(Views.automationEdit);

		this.setState({
			loading: false,
			data,
			fields: dataView?.fields || [],
		});
	};

	handleSubmitClick = async (e: React.MouseEvent<any>): Promise<void> => {
		await this._formRef.current.submit();
	};

	handleSubmit = async (values: Automation): Promise<ApiResult> => {
		const { entityTypeCode, entityUid, uid, onSuccess } = this.props;

		const item = { ...values };

		let result;

		if (uid) {
			result = await this._automationService.update({ entityTypeCode, entityUid, item: { uid, ...item } });
		}
		else {
			result = await this._automationService.insert({ entityTypeCode, entityUid, item });
		}

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
				<Drawer
					title="Automation"
					closable={true}
					onClose={onClose}
					visible={true}
					width={800}
					footer={
						<Toolbar clear size="small" float="right">
							<ButtonCancel onClick={onClose} />
							<ButtonSave onClick={this.handleSubmitClick} />
						</Toolbar>}>

					{/* todo: pass Automation to context? */}
					<AutomationContextProvider entityTypeCode={entityTypeCode} entityUid={entityUid}>
						<DataForm
							formRef={this._formRef}
							hideButtons={true}
							fields={fields}
							data={data}
							onSubmit={this.handleSubmit} />
					</AutomationContextProvider>

				</Drawer>
			</Spin>
		</>);
	};
}
