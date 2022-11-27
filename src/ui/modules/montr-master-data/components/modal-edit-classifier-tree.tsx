import { DataForm } from "@montr-core/components/data-form";
import { ApiResult, Guid, IDataField } from "@montr-core/models";
import { MetadataService, NotificationService } from "@montr-core/services";
import { Modal, Spin } from "antd";
import { FormInstance } from "antd/es/form";
import * as React from "react";
import { ClassifierGroup } from "../models/classifier-group";
import { ClassifierTreeService } from "../services/classifier-tree-service";

interface Props {
	typeCode: string;
	uid?: Guid;
	onSuccess?: (data: ClassifierGroup) => void;
	onCancel?: () => void;
}

interface State {
	loading: boolean;
	fields?: IDataField[];
	data: ClassifierGroup;
}

export class ModalEditClassifierTree extends React.Component<Props, State> {
	private readonly notificationService = new NotificationService();
	private readonly metadataService = new MetadataService();
	private readonly classifierTreeService = new ClassifierTreeService();

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
		await this.classifierTreeService.abort();
	};

	fetchData = async (): Promise<void> => {
		const { typeCode, uid } = this.props;

		try {

			const dataView = await this.metadataService.load(`ClassifierTree/Form`);

			const fields = dataView.fields;

			let data;

			if (uid) {
				data = await this.classifierTreeService.get(typeCode, uid);
			}
			else {
				// todo: load defaults from server
				data = {};
			}

			this.setState({ loading: false, fields, data });

		} catch (error) {
			this.notificationService.error("Ошибка при загрузке данных", error.message);
			// todo: why call onCancel()?
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
		const { typeCode, uid, onSuccess } = this.props;

		let data: ClassifierGroup,
			result: ApiResult;

		if (uid) {
			data = { uid: uid, ...values };

			result = await this.classifierTreeService.update(typeCode, data);
		}
		else {
			const insertResult = await this.classifierTreeService.insert(typeCode, values);

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
			<Modal open={!loading} title={data.name}
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
