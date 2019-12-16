import * as React from "react";
import { Modal, Spin } from "antd";
import { IClassifierLink } from "../models";
import { ClassifierLinkService } from "../services";
import { IDataField, IApiResult, IClassifierField, Guid } from "@montr-core/models";
import { WrappedDataForm, DataForm } from "@montr-core/components";
import { CompanyContextProps, withCompanyContext } from "@montr-kompany/components";
import { NotificationService, MetadataService } from "@montr-core/services";

interface IProps extends CompanyContextProps {
	typeCode: string;
	itemUid: Guid;
	onSuccess?: (data: IClassifierLink) => void;
	onCancel?: () => void;
}

interface IState {
	loading: boolean;
	fields?: IDataField[];
	data: IClassifierLink;
}

class _ModalEditClassifierLink extends React.Component<IProps, IState> {
	private _notificationService = new NotificationService();
	private _metadataService = new MetadataService();
	private _classifierLinkService = new ClassifierLinkService();

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
		await this._classifierLinkService.abort();
	}

	fetchData = async () => {
		const { currentCompany, typeCode } = this.props;

		if (currentCompany) {

			try {
				const dataView = await this._metadataService.load(`ClassifierLink/Form`);

				const fields = dataView.fields;

				const classifierField = fields.find(x => x.key == "group.uid") as IClassifierField;

				if (classifierField) {
					classifierField.typeCode = typeCode;
				}

				this.setState({ loading: false, fields });

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

	save = async (values: IClassifierLink): Promise<IApiResult> => {
		const { typeCode, itemUid, onSuccess } = this.props;

		const result = await this._classifierLinkService.insert(typeCode, values.group.uid, itemUid);

		if (result.success) {
			if (onSuccess) await onSuccess(values);
		}

		return result;
	}

	render = () => {
		const { loading, fields, data } = this.state;

		return (
			<Modal visible={!loading} title="Добавление ссылки"
				onOk={this.onOk} onCancel={this.onCancel}
				okText="Сохранить" width="640px">
				<Spin spinning={loading}>
					<DataForm
						wrappedComponentRef={this.saveFormRef}
						fields={fields}
						data={data}
						showControls={false}
						onSubmit={this.save}
					/>
				</Spin>
			</Modal>
		);
	}
}

export const ModalEditClassifierLink = withCompanyContext(_ModalEditClassifierLink);
