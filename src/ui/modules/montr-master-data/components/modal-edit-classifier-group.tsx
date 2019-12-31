import * as React from "react";
import { Modal, Spin } from "antd";
import { Guid, IDataField, IApiResult, IClassifierGroupField } from "@montr-core/models";
import { withCompanyContext, CompanyContextProps } from "@montr-kompany/components";
import { ClassifierGroupService } from "../services";
import { IClassifierGroup } from "@montr-master-data/models";
import { NotificationService, MetadataService } from "@montr-core/services";
import { DataForm, WrappedDataForm } from "@montr-core/components";

interface IProps extends CompanyContextProps {
	typeCode: string;
	treeUid: Guid;
	uid?: Guid;
	parentUid?: Guid;
	hideFields?: string[];
	onSuccess?: (data: IClassifierGroup) => void;
	onCancel?: () => void;
}

interface IState {
	loading: boolean;
	fields?: IDataField[];
	data: IClassifierGroup;
}

class _ModalEditClassifierGroup extends React.Component<IProps, IState> {
	private _notificationService = new NotificationService();
	private _metadataService = new MetadataService();
	private _classifierGroupService = new ClassifierGroupService();

	private _formRef: WrappedDataForm;

	constructor(props: IProps) {
		super(props);

		this.state = {
			loading: true,
			data: {}
		};
	}

	componentDidMount = async () => {
		await this.fetchData();
	};

	componentWillUnmount = async () => {
		await this._metadataService.abort();
		await this._classifierGroupService.abort();
	};

	fetchData = async () => {
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

	saveFormRef = (formRef: WrappedDataForm) => {
		this._formRef = formRef;
	};

	onOk = async (e: React.MouseEvent<any>) => {
		await this._formRef.handleSubmit(e);
	};

	onCancel = () => {
		if (this.props.onCancel) this.props.onCancel();
	};

	save = async (values: IClassifierGroup): Promise<IApiResult> => {
		const { typeCode, treeUid, uid, onSuccess } = this.props;

		let data: IClassifierGroup,
			result: IApiResult;

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

	render = () => {
		const { loading, fields, data } = this.state;

		return (
			<Modal visible={!loading} title={data.name}
				onOk={this.onOk} onCancel={this.onCancel}
				okText="Сохранить" width="640px">
				<Spin spinning={loading}>
					<DataForm
						layout="vertical"
						wrappedComponentRef={this.saveFormRef}
						fields={fields}
						data={data}
						showControls={false}
						onSubmit={this.save}
					/>
				</Spin>
			</Modal>
		);
	};
}

export const ModalEditClassifierGroup = withCompanyContext(_ModalEditClassifierGroup);
