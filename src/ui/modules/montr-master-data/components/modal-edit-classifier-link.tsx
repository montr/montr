import { DataForm } from "@montr-core/components/data-form";
import { ApiResult, Guid, IDataField } from "@montr-core/models";
import { MetadataService, NotificationService } from "@montr-core/services";
import { Modal, Spin } from "antd";
import { FormInstance } from "antd/es/form";
import * as React from "react";
import { ClassifierLink, IClassifierField } from "../models";
import { ClassifierLinkService } from "../services/classifier-link-service";

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
	private readonly notificationService = new NotificationService();
	private readonly metadataService = new MetadataService();
	private readonly classifierLinkService = new ClassifierLinkService();

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
		await this.metadataService.abort();
		await this.classifierLinkService.abort();
	};

	fetchData = async (): Promise<void> => {
		const { typeCode } = this.props;

		try {
			const dataView = await this.metadataService.load(`ClassifierLink/Form`);

			const fields = dataView.fields;

			const classifierField = fields.find(x => x.key == "group.uid") as IClassifierField;

			if (classifierField) {
				classifierField.props = { typeCode };
			}

			this.setState({ loading: false, fields });

		} catch (error) {
			this.notificationService.error("Ошибка при загрузке данных", error.message);
			this.onCancel();
		}
	};

	onOk = async (e: React.MouseEvent<any>): Promise<void> => {
		await this._formRef.current.submit();
	};

	onCancel = (): void => {
		if (this.props.onCancel) this.props.onCancel();
	};

	save = async (values: ClassifierLink): Promise<ApiResult> => {
		const { typeCode, itemUid, onSuccess } = this.props;

		const result = await this.classifierLinkService.insert(typeCode, values.group.uid, itemUid);

		if (result.success) {
			if (onSuccess) await onSuccess(values);
		}

		return result;
	};

	render = (): React.ReactNode => {
		const { loading, fields, data } = this.state;

		return (
			<Modal open={!loading} title="Добавление ссылки"
				onOk={this.onOk} onCancel={this.onCancel}
				okText="Сохранить" width="640px">
				<Spin spinning={loading}>
					<DataForm
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
