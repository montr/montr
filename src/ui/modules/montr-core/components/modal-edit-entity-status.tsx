import { EntityStatusService } from "@montr-core/services/entity-status-service";
import { Modal, Spin } from "antd";
import { FormInstance } from "antd/es/form";
import React from "react";
import { ApiResult, EntityStatus, Guid, IDataField } from "../models";
import { Views } from "../module";
import { MetadataService } from "../services/metadata-service";
import { DataForm } from "./data-form";

interface Props {
	entityTypeCode: string;
	entityUid: Guid;
	uid?: Guid;
	onSuccess?: (data: EntityStatus) => void;
	onCancel?: () => void;
}

interface State {
	loading: boolean;
	fields?: IDataField[];
	data: Partial<EntityStatus>;
}

export class ModalEditEntityStatus extends React.Component<Props, State> {

	private readonly metadataService = new MetadataService();
	private readonly entityStatusService = new EntityStatusService();

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
		await this.metadataService.abort();
		await this.entityStatusService.abort();
	};

	fetchData = async () => {
		const { entityTypeCode, entityUid, uid } = this.props;

		const dataView = await this.metadataService.load(Views.entityStatusForm);

		const fields = dataView.fields;

		let data;

		if (uid) {
			data = await this.entityStatusService.get({ entityTypeCode, entityUid, uid });
		}
		else {
			// todo: load defaults from server
			data = {};
		}

		this.setState({ loading: false, fields, data });
	};

	onOk = async (e: React.MouseEvent<any>) => {
		await this._formRef.current.submit();
	};

	onCancel = () => {
		if (this.props.onCancel) this.props.onCancel();
	};

	save = async (values: EntityStatus): Promise<ApiResult> => {
		const { entityTypeCode, entityUid, uid, onSuccess } = this.props;

		let data: EntityStatus,
			result: ApiResult;

		if (uid) {
			data = { uid: uid, ...values };

			result = await this.entityStatusService.update({
				entityTypeCode, entityUid, item: data
			});
		}
		else {
			const insertResult = await this.entityStatusService.insert({
				entityTypeCode, entityUid, item: values
			});

			// todo: reload from server?
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
