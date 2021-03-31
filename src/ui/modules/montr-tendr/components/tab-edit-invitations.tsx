import * as React from "react";
import { Button, Alert } from "antd";
import { PaneProps, Guid, DataResult, IMenu } from "@montr-core/models";
import { DataTable, Toolbar, DataTableUpdateToken, ButtonAdd, ButtonDelete, Icon } from "@montr-core/components";
import { PaneSelectClassifier } from "@montr-master-data/components";
import { ModalEditInvitation } from "../components";
import { IEvent, Invitation } from "../models";
import { InvitationService } from "../services";
import { Constants } from "@montr-core/.";
import { OperationService } from "@montr-core/services";

interface Props extends PaneProps<IEvent> {
	data: IEvent;
}

interface State {
	showDrawer?: boolean;
	editData?: Invitation;
	selectedRowKeys?: string[] | number[];
	updateTableToken: DataTableUpdateToken;
}

export default class TabEditInvitations extends React.Component<Props, State> {

	private _operation = new OperationService();
	private _invitationService = new InvitationService();

	constructor(props: Props) {
		super(props);

		this.state = {
			updateTableToken: { date: new Date() }
		};
	}

	componentWillUnmount = async (): Promise<void> => {
		await this._invitationService.abort();
	};

	onLoadTableData = async (loadUrl: string, postParams: any): Promise<DataResult<{}> | undefined> => {
		const { data } = this.props;

		if (data) {

			const params = {
				eventUid: data.uid,
				...postParams
			};

			return await this._invitationService.post(loadUrl, params);
		}

		return undefined;
	};

	onSelectionChange = async (selectedRowKeys: string[] | number[]): Promise<void> => {
		this.setState({ selectedRowKeys });
	};

	refreshTable = async (resetSelectedRows?: boolean): Promise<void> => {
		const { selectedRowKeys } = this.state;

		this.setState({
			updateTableToken: { date: new Date(), resetSelectedRows },
			selectedRowKeys: resetSelectedRows ? [] : selectedRowKeys
		});
	};

	showAddDrawer = async (): Promise<void> => {
		this.setState({ showDrawer: true });
	};

	onCloseDrawer = async (): Promise<void> => {
		this.setState({ showDrawer: false });
	};

	onSelect = async (keys: string[]): Promise<void> => {
		const { data } = this.props;

		if (data.uid) {
			const result = await this._invitationService.insert({
				eventUid: data.uid,
				items: keys.map(x => {
					return { counterpartyUid: new Guid(x) };
				})
			});
		}

		await this.onCloseDrawer();

		await this.refreshTable();
	};

	showAddModal = async (): Promise<void> => {
		this.setState({ editData: {} });
	};

	showEditModal = async (data: Invitation): Promise<void> => {
		this.setState({ editData: data });
	};

	onModalSuccess = async (data: Invitation): Promise<void> => {
		this.setState({ editData: undefined });

		await this.refreshTable();
	};

	onModalCancel = async (): Promise<void> => {
		this.setState({ editData: undefined });
	};

	delete = async (): Promise<void> => {
		const { selectedRowKeys } = this.state;

		if (selectedRowKeys) {
			await this._operation.execute(async () => {
				const result = await this._invitationService.delete(selectedRowKeys);
				if (result.success) {
					this.refreshTable(true);
				}
				return result;

			}, {
				showConfirm: true,
				confirmTitle: "Вы действительно хотите удалить выбранные приглашения?"
			});
		}
	};

	render = (): React.ReactNode => {
		const { data } = this.props,
			{ selectedRowKeys, updateTableToken, editData, showDrawer } = this.state;

		const rowActions: IMenu[] = [
			{ name: "Редактировать", onClick: this.showEditModal }
		];

		return <>
			<Toolbar clear>
				<Button icon={Icon.Plus} onClick={this.showAddDrawer} type="primary">Пригласить</Button>
				<ButtonAdd onClick={this.showAddModal} />
				<ButtonDelete onClick={this.delete} disabled={!selectedRowKeys?.length} />
			</Toolbar>

			<DataTable
				rowKey="uid"
				viewId="Event/Invitation/List"
				loadUrl={`${Constants.apiURL}/invitation/list/`}
				rowActions={rowActions}
				onLoadData={this.onLoadTableData}
				onSelectionChange={this.onSelectionChange}
				updateToken={updateTableToken}
			/>

			<p />

			<Alert type="info" message={
				<ul>
					<li>Manual add</li>
					<li>Import from *.xls etc</li>
					<li>Select from registered companies</li>
					<li>Invite from companies catalogs</li>
					<li>👍 Select from counterparty classifier</li>
					<li>Copy invitation from other event</li>
				</ul>
			} />

			{data.uid && editData &&
				<ModalEditInvitation
					eventUid={data.uid}
					uid={editData.uid}
					onSuccess={this.onModalSuccess}
					onCancel={this.onModalCancel}
				/>}

			{showDrawer && <PaneSelectClassifier
				typeCode="counterparty"
				onSelect={this.onSelect}
				onClose={this.onCloseDrawer}
			/>}

		</>;
	};
}
