import { ButtonAdd, ButtonDelete, Toolbar } from "@montr-core/components";
import { DataTable, DataTableUpdateToken } from "@montr-core/components/data-table";
import { DataResult } from "@montr-core/models";
import { OperationService } from "@montr-core/services";
import { PaneSelectClassifier } from "@montr-master-data/components";
import React from "react";
import { Role, User } from "../models";
import { Api, Views } from "../module";
import { UserRoleService } from "../services";

interface Props {
	data: User;
}

interface State {
	showDrawer?: boolean;
	selectedRowKeys?: string[] | number[];
	selectedRows?: Role[];
	updateTableToken: DataTableUpdateToken;
}

export default class TabEditUserRoles extends React.Component<Props, State> {

	private readonly operation = new OperationService();
	private readonly userRoleService = new UserRoleService();

	constructor(props: Props) {
		super(props);

		this.state = {
			updateTableToken: { date: new Date() }
		};
	}

	componentWillUnmount = async (): Promise<void> => {
		await this.userRoleService.abort();
	};

	onSelectionChange = async (selectedRowKeys: string[], selectedRows: Role[]): Promise<void> => {
		this.setState({ selectedRowKeys, selectedRows });
	};

	refreshTable = (resetCurrentPage?: boolean, resetSelectedRows?: boolean): void => {
		const { selectedRowKeys } = this.state;

		this.setState({
			updateTableToken: { date: new Date(), resetCurrentPage, resetSelectedRows },
			selectedRowKeys: resetSelectedRows ? [] : selectedRowKeys
		});
	};

	onLoadTableData = async (loadUrl: string, postParams: any): Promise<DataResult<Role>> => {
		const { data } = this.props;

		const params = { userUid: data?.uid, ...postParams };

		return await this.userRoleService.post(loadUrl, params);
	};

	onSelect = async (_keys: string[], rows: Role[]): Promise<void> => {
		const { data } = this.props;

		if (data.uid) {
			const result = await this.operation.execute(async () => {
				return await this.userRoleService.addRoles({
					userUid: data.uid,
					roles: rows.map(x => x.name)
				});
			});

			if (result.success) {
				await this.onCloseDrawer();
				await this.refreshTable();
			}
		}
	};

	showAddDrawer = async (): Promise<void> => {
		this.setState({ showDrawer: true });
	};

	onCloseDrawer = async (): Promise<void> => {
		this.setState({ showDrawer: false });
	};

	deleteSelectedRows = async (): Promise<void> => {
		const { data } = this.props,
			{ selectedRows } = this.state;

		if (selectedRows) {
			await this.operation.confirmDelete(async () => {
				const result = await this.userRoleService.removeRoles({
					userUid: data.uid,
					roles: selectedRows.map(x => x.name)
				});

				if (result.success) {
					this.refreshTable(true, true);
				}

				return result;
			});
		}
	};

	render = (): React.ReactNode => {

		const { selectedRowKeys, showDrawer, updateTableToken } = this.state;

		return (<>
			<Toolbar clear>
				<ButtonAdd onClick={this.showAddDrawer} type="primary" />
				<ButtonDelete onClick={this.deleteSelectedRows} disabled={!selectedRowKeys?.length} />
			</Toolbar>

			<DataTable
				rowKey="uid"
				viewId={Views.userRolesGrid}
				loadUrl={Api.userRoleList}
				onLoadData={this.onLoadTableData}
				onSelectionChange={this.onSelectionChange}
				updateToken={updateTableToken}
			/>

			{showDrawer && <PaneSelectClassifier
				typeCode="role"
				onSelect={this.onSelect}
				onClose={this.onCloseDrawer}
			/>}
		</>);
	};
}
