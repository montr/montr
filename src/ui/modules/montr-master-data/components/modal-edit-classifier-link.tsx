import * as React from "react";
import { Modal, Spin } from "antd";
import { FormInstance } from "antd/lib/form";
import { ClassifierLink, IClassifierField } from "../models";
import { ClassifierLinkService } from "../services";
import { IDataField, ApiResult, Guid } from "@montr-core/models";
import { DataForm } from "@montr-core/components";
import { NotificationService, MetadataService } from "@montr-core/services";

interface Props {
	typeCode: string;
	itemUid: Guid;
	onSuccess?: (data: ClassifierLink) => void;
	onCancel?: () => void;
}

interface State {
	loading: boolean;
	fields?: IDataField[];
	data: ClassifierLink;
}

export class ModalEditClassifierLink extends React.Component<Props, State> {
	private _notificationService = new NotificationService();
	private _metadataService = new MetadataService();
	private _classifierLinkService = new ClassifierLinkService();

	private _formRef = React.createRef<FormInstance>();

	constructor(props: Props) {
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
		await this._classifierLinkService.abort();
	};

	fetchData = async () => {
		const { typeCode } = this.props;

		try {
			const dataView = await this._metadataService.load(`ClassifierLink/Form`);

			const fields = dataView.fields;

			const classifierField = fields.find(x => x.key == "group.uid") as IClassifierField;

			if (classifierField) {
				classifierField.props = { typeCode };
			}

			this.setState({ loading: false, fields });

		} catch (error) {
			this._notificationService.error("Ошибка при загрузке данных", error.message);
			this.onCancel();
		}
	};

	onOk = async (e: React.MouseEvent<any>) => {
		await this._formRef.current.submit();
	};

	onCancel = () => {
		if (this.props.onCancel) this.props.onCancel();
	};

	save = async (values: ClassifierLink): Promise<ApiResult> => {
		const { typeCode, itemUid, onSuccess } = this.props;

		const result = await this._classifierLinkService.insert(typeCode, values.group.uid, itemUid);

		if (result.success) {
			if (onSuccess) await onSuccess(values);
		}

		return result;
	};

	render = () => {
		const { loading, fields, data } = this.state;

		return (
			<Modal visible={!loading} title="Добавление ссылки"
				onOk={this.onOk} onCancel={this.onCancel}
				okText="Сохранить" width="640px">
				<Spin spinning={loading}>
					<DataForm
						formRef={this._formRef}
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
