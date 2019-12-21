import * as React from "react";
import { Guid, IDataField, IApiResult } from "@montr-core/models";
import { CompanyContextProps, withCompanyContext } from "@montr-kompany/components";
import { IInvitation } from "../models";
import { Modal, Spin } from "antd";
import { DataForm } from "@montr-core/components";
import { NotificationService, MetadataService } from "@montr-core/services";
import { InvitationService } from "../services";
import { FormInstance } from "antd/lib/form";

interface IProps extends CompanyContextProps {
	eventUid: Guid;
	uid?: Guid;
	onSuccess?: (data: IInvitation) => void;
	onCancel?: () => void;
}

interface IState {
	loading: boolean;
	fields?: IDataField[];
	data: IInvitation;
}

class _ModalEditInvitation extends React.Component<IProps, IState> {

	private _notificationService = new NotificationService();
	private _metadataService = new MetadataService();
	private _invitationService = new InvitationService();

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
		await this._invitationService.abort();
	};

	fetchData = async () => {
		const { currentCompany, uid } = this.props;

		if (currentCompany) {

			try {

				const dataView = await this._metadataService.load(`Event/Invitation/Form`);

				const fields = dataView.fields;

				let data;

				if (uid) {
					data = await this._invitationService.get(currentCompany.uid, uid);
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
		}
	};

	onOk = async (e: React.MouseEvent<any>) => {
		await this._formRef.current.submit();
	};

	onCancel = () => {
		if (this.props.onCancel) this.props.onCancel();
	};

	save = async (values: IInvitation): Promise<IApiResult> => {
		const { eventUid, uid, onSuccess } = this.props,
			{ uid: companyUid } = this.props.currentCompany;

		let data: IInvitation,
			result: IApiResult;

		if (uid) {
			data = { uid: uid, ...values };

			result = await this._invitationService.update(companyUid, { eventUid, item: data });
		}
		else {
			const insertResult =
				await this._invitationService.insert(companyUid, { eventUid, items: [values] });

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
			<Modal visible={!loading}
				// title={data.name}
				title="Приглашение"
				onOk={this.onOk}
				onCancel={this.onCancel}
				okText="Сохранить"
				width="640px">
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

export const ModalEditInvitation = withCompanyContext(_ModalEditInvitation);
