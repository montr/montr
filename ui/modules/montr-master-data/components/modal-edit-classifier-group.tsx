import * as React from "react";
import { Modal, Spin } from "antd";
import { Guid, IFormField, IClassifierField } from "@montr-core/models";
import { withCompanyContext, CompanyContextProps } from "@kompany/components";
import { ClassifierGroupService } from "../services";
import { IClassifierGroup } from "@montr-master-data/models";
import { NotificationService, MetadataService } from "@montr-core/services";
import { DataForm, WrappedDataForm } from "@montr-core/components";

interface IProps extends CompanyContextProps {
	typeCode: string;
	treeCode: string,
	uid?: Guid;
	parentUid?: Guid;
	onSuccess?: (data: IClassifierGroup) => void
	onCancel?: () => void;
}

interface IState {
	loading: boolean;
	fields?: IFormField[];
	data: IClassifierGroup;
	// parent?: IClassifierGroup;
}

class _ModalEditClassifierGroup extends React.Component<IProps, IState> {
	private _notificationService = new NotificationService();
	private _metadataService = new MetadataService();
	private _classifierGroupService = new ClassifierGroupService();

	_formRef: WrappedDataForm;

	constructor(props: IProps) {
		super(props);

		this.state = {
			loading: true,
			data: {}
		};
	}

	componentDidMount = async () => {
		await this.fetchData();
	}

	componentWillUnmount = async () => {
		await this._metadataService.abort();
		await this._classifierGroupService.abort();
	}

	fetchData = async () => {
		const { currentCompany, typeCode, treeCode, uid } = this.props;

		if (currentCompany) {

			try {

				const dataView = await this._metadataService.load(`ClassifierGroup/${typeCode}`);

				const parentUidField = dataView.fields.find(x => x.key == "parentUid") as IClassifierField;
				parentUidField.typeCode = typeCode;
				parentUidField.treeCode = treeCode;

				let data, /* parent, */ { parentUid } = this.props;

				if (uid) {
					data = await this._classifierGroupService.get(currentCompany.uid, typeCode, treeCode, uid);
					// parentUid = data.parentUid;
				}
				else {
					data = { parentUid: parentUid };
				}

				/* if (parentUid) {
					parent = await this._classifierGroupService.get(currentCompany.uid, typeCode, treeCode, parentUid);
				} */

				this.setState({ loading: false, fields: dataView.fields, data/* : data || {}, parent */ });

			} catch (error) {
				this._notificationService.error("Ошибка при загрузке данных", error.message);
				this.onCancel();
			}
		}
	}

	saveFormRef = (formRef: WrappedDataForm) => {
		this._formRef = formRef;
	}

	onOk = async (e: React.MouseEvent<any>) => {
		await this._formRef.handleSubmit(e);
	}

	onCancel = () => {
		if (this.props.onCancel) this.props.onCancel();
	}

	save = async (values: IClassifierGroup) => {
		const { typeCode, treeCode, uid, onSuccess } = this.props;
		const { uid: companyUid } = this.props.currentCompany;

		let data: IClassifierGroup;
		if (uid) {
			data = { uid: uid, ...values };
			const rowsUpdated = await this._classifierGroupService.update(companyUid, data);

			if (rowsUpdated != 1) throw new Error();
		}
		else {
			const uid: Guid = await this._classifierGroupService.insert(companyUid, typeCode, treeCode, values);
			data = { uid: uid, ...values };
		}

		if (onSuccess) await onSuccess(data);
	}

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
						onSave={this.save}
					/>
				</Spin>
			</Modal>
		);
	}
}

export const ModalEditClassifierGroup = withCompanyContext(_ModalEditClassifierGroup);
