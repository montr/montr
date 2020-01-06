import * as React from "react";
import { Modal, Spin } from "antd";
import { Guid, IDataField, IApiResult } from "@montr-core/models";
import { withCompanyContext, CompanyContextProps } from "@montr-kompany/components";
import { ClassifierTreeService } from "../services";
import { IClassifierGroup } from "@montr-master-data/models";
import { NotificationService, MetadataService } from "@montr-core/services";
import { DataForm, WrappedDataForm } from "@montr-core/components";
import { FormInstance } from "antd/lib/form";

interface IProps extends CompanyContextProps {
	typeCode: string;
	uid?: Guid;
	onSuccess?: (data: IClassifierGroup) => void;
	onCancel?: () => void;
}

interface IState {
	loading: boolean;
	fields?: IDataField[];
	data: IClassifierGroup;
}

class _ModalEditClassifierTree extends React.Component<IProps, IState> {
	private _notificationService = new NotificationService();
	private _metadataService = new MetadataService();
	private _classifierTreeService = new ClassifierTreeService();

	private _formRef = React.createRef<FormInstance>();

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
		await this._classifierTreeService.abort();
	};

	fetchData = async () => {
		const { typeCode, uid } = this.props;

		try {

			const dataView = await this._metadataService.load(`ClassifierTree/Form`);

			const fields = dataView.fields;

			let data;

			if (uid) {
				data = await this._classifierTreeService.get(typeCode, uid);
			}
			else {
				// todo: load defaults from server
				data = {};
			}

			this.setState({ loading: false, fields, data });

		} catch (error) {
			this._notificationService.error("Ошибка при загрузке данных", error.message);
			// todo: why call onCancel()?
			this.onCancel();
		}
	};

	onOk = async (e: React.MouseEvent<any>) => {
		await this._formRef.current.submit();
	};

	onCancel = () => {
		if (this.props.onCancel) this.props.onCancel();
	};

	save = async (values: IClassifierGroup): Promise<IApiResult> => {
		const { typeCode, uid, onSuccess } = this.props;

		let data: IClassifierGroup,
			result: IApiResult;

		if (uid) {
			data = { uid: uid, ...values };

			result = await this._classifierTreeService.update(typeCode, data);
		}
		else {
			const insertResult = await this._classifierTreeService.insert(typeCode, values);

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

export const ModalEditClassifierTree = withCompanyContext(_ModalEditClassifierTree);
