import * as React from "react";
import { Modal, Spin } from "antd";
import { FormInstance } from "antd/lib/form";
import { Guid, IDataField, ApiResult } from "@montr-core/models";
import { ClassifierGroupService } from "../services";
import { ClassifierGroup, IClassifierGroupField } from "../models";
import { NotificationService, MetadataService } from "@montr-core/services";
import { DataForm } from "@montr-core/components";

interface Props {
	typeCode: string;
	treeUid: Guid;
	uid?: Guid;
	parentUid?: Guid;
	hideFields?: string[];
	onSuccess?: (data: ClassifierGroup) => void;
	onCancel?: () => void;
}

interface State {
	loading: boolean;
	fields?: IDataField[];
	data: ClassifierGroup;
}

export class ModalEditClassifierGroup extends React.Component<Props, State> {
	private _notificationService = new NotificationService();
	private _metadataService = new MetadataService();
	private _classifierGroupService = new ClassifierGroupService();

	private _formRef = React.createRef<FormInstance>();

	constructor(props: Props) {
		super(props);

		this.state = {
			loading: true,
			data: {}
		};
	}

	componentDidMount = async (): Promise<void> => {
		await this.fetchData();
	};

	componentWillUnmount = async (): Promise<void> => {
		await this._metadataService.abort();
		await this._classifierGroupService.abort();
	};

	fetchData = async (): Promise<void> => {
		const { typeCode, treeUid, uid, parentUid, hideFields } = this.props;

		try {

			const dataView = await this._metadataService.load(`ClassifierGroup/Form`);

			const fields = hideFields
				? dataView.fields.filter(x => hideFields.includes(x.key) == false)
				: dataView.fields;

			const parentUidField = fields.find(x => x.key == "parentUid") as IClassifierGroupField;

			if (parentUidField) {
				parentUidField.props = { typeCode, treeUid };
			}

			let data;

			if (uid) {
				data = await this._classifierGroupService.get(typeCode, treeUid, uid);
			}
			else {
				// todo: load defaults from server
				data = { parentUid: parentUid };
			}

			this.setState({ loading: false, fields, data });

		} catch (error) {
			this._notificationService.error("Ошибка при загрузке данных", error.message);
			this.onCancel();
		}
	};

	onOk = async (e: React.MouseEvent<any>): Promise<void> => {
		await this._formRef.current.submit();
	};

	onCancel = (): void => {
		if (this.props.onCancel) this.props.onCancel();
	};

	save = async (values: ClassifierGroup): Promise<ApiResult> => {
		const { typeCode, treeUid, uid, onSuccess } = this.props;

		let data: ClassifierGroup,
			result: ApiResult;

		if (uid) {
			data = { uid: uid, ...values };

			result = await this._classifierGroupService.update(typeCode, data);
		}
		else {
			const insertResult = await this._classifierGroupService.insert(typeCode, treeUid, values);

			data = { uid: insertResult.uid, ...values };

			result = insertResult;
		}

		if (result.success) {
			if (onSuccess) await onSuccess(data);
		}

		return result;
	};

	render = (): React.ReactNode => {
		const { loading, fields, data } = this.state;

		return (
			<Modal visible={!loading} title={data.name}
				onOk={this.onOk} onCancel={this.onCancel}
				okText="Сохранить" width="640px">
				<Spin spinning={loading}>
					<DataForm
						layout="vertical"
						formRef={this._formRef}
						fields={fields}
						data={data}
						hideButtons={true}
						onSubmit={this.save}
					/>
				</Spin>
			</Modal>
		);
	};
}
