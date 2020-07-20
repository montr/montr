import React from "react";
import { Spin, Drawer } from "antd";
import { FormInstance } from "antd/lib/form";
import { Guid, IApiResult, IDataField } from "@montr-core/models";
import { Toolbar, ButtonCancel, ButtonSave, DataForm } from "@montr-core/components";
import { Automation } from "../models";
import { MetadataService } from "@montr-core/services";
import { AutomationService } from "../services";
import { AutomationContextProvider } from "./automation-context";

interface Props {
	entityTypeCode: string;
	entityTypeUid: Guid | string;
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

	componentDidMount = async () => {
		await this.fetchData();
	};

	componentWillUnmount = async () => {
		await this._metadataService.abort();
		await this._automationService.abort();
	};

	fetchData = async () => {
		const { entityTypeCode, entityTypeUid, uid } = this.props;

		const data: Automation = (uid)
			? await this._automationService.get(entityTypeCode, entityTypeUid, uid)
			// todo: load defaults from server
			: {
				conditions: [],
				actions: []
			};

		const dataView = await this._metadataService.load("Automation/Edit");

		this.setState({
			loading: false,
			data,
			fields: dataView?.fields || [],
		});
	};

	handleSubmitClick = async (e: React.MouseEvent<any>) => {
		await this._formRef.current.submit();
	};

	handleSubmit = async (values: Automation): Promise<IApiResult> => {
		const { entityTypeCode, entityTypeUid, uid, onSuccess } = this.props;

		const item = { ...values };

		let result;

		if (uid) {
			result = await this._automationService.update({ entityTypeCode, entityTypeUid, item: { uid, ...item } });
		}
		else {
			result = await this._automationService.insert({ entityTypeCode, entityTypeUid, item });
		}

		if (result.success && onSuccess) {
			onSuccess();
		}

		return result;
	};

	render = () => {
		const { entityTypeCode, entityTypeUid, onClose } = this.props,
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

					{/* todo: pass Autmation to context? */}
					<AutomationContextProvider entityTypeCode={entityTypeCode} entityTypeUid={entityTypeUid}>
						<DataForm
							formRef={this._formRef}
							showControls={false}
							fields={fields}
							data={data}
							onSubmit={this.handleSubmit} />
					</AutomationContextProvider>

				</Drawer>
			</Spin>
		</>);
	};
}
