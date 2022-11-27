import { DataForm } from "@montr-core/components";
import { ApiResult, Guid, IDataField } from "@montr-core/models";
import { MetadataService, NotificationService } from "@montr-core/services";
import { Modal, Spin } from "antd";
import { FormInstance } from "antd/es/form";
import * as React from "react";
import { Invitation } from "../models";
import { InvitationService } from "../services";

interface Props {
	eventUid: Guid;
	uid?: Guid;
	onSuccess?: (data: Invitation) => void;
	onCancel?: () => void;
}

interface State {
	loading: boolean;
	fields?: IDataField[];
	data: Invitation;
}

export class ModalEditInvitation extends React.Component<Props, State> {

	private readonly notificationService = new NotificationService();
	private readonly metadataService = new MetadataService();
	private readonly invitationService = new InvitationService();

	private readonly formRef = React.createRef<FormInstance>();

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
		await this.invitationService.abort();
	};

	fetchData = async (): Promise<void> => {
		const { uid } = this.props;

		try {

			const dataView = await this.metadataService.load(`Event/Invitation/Form`);

			const fields = dataView.fields;

			let data;

			if (uid) {
				data = await this.invitationService.get(uid);
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

	onOk = async (): Promise<void> => {
		const form = this.formRef.current;
		if (form) {
			await form.submit();
		}
	};

	onCancel = async (): Promise<void> => {
		if (this.props.onCancel) this.props.onCancel();
	};

	save = async (values: Invitation): Promise<ApiResult> => {
		const { eventUid, uid, onSuccess } = this.props;

		let data: Invitation,
			result: ApiResult;

		if (uid) {
			data = { uid: uid, ...values };

			result = await this.invitationService.update({ eventUid, item: data });
		}
		else {
			const insertResult =
				await this.invitationService.insert({ eventUid, items: [values] });

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
			<Modal open={!loading}
				// title={data.name}
				title="Приглашение"
				onOk={this.onOk}
				onCancel={this.onCancel}
				okText="Сохранить"
				width="640px">
				<Spin spinning={loading}>
					<DataForm
						formRef={this.formRef}
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
