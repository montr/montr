import React from "react";
import { Translation } from "react-i18next";
import { DataTableUpdateToken, Toolbar, ButtonAdd, ButtonDelete, DataTable } from "@montr-core/components";
import { Guid, IDataResult } from "@montr-core/models";
import { OperationService } from "@montr-core/services";
import { Constants } from "@montr-core/.";
import i18next from "i18next";
import { AutomationService } from "../services/automation-service";
import { Automation } from "../models/automation";
import { PaneEditAutomation } from "./pane-edit-automation";

interface IProps {
	entityTypeCode: string;
	entityTypeUid: Guid | string;
}

interface IState {
	showPane?: boolean;
	editUid?: Guid;
	selectedRowKeys?: string[] | number[];
	updateTableToken?: DataTableUpdateToken;
}

export class PaneSearchAutomation extends React.Component<IProps, IState> {

	private _operation = new OperationService();
	private _automationService = new AutomationService();

	constructor(props: IProps) {
		super(props);

		this.state = {
		};
	}

	componentDidMount = async () => {
	};

	componentWillUnmount = async () => {
		await this._automationService.abort();
	};

	onLoadTableData = async (loadUrl: string, postParams: any): Promise<IDataResult<{}>> => {
		const { entityTypeCode, entityTypeUid } = this.props;

		const params = { entityTypeCode, entityTypeUid, ...postParams };

		return await this._automationService.post(loadUrl, params);
	};

	onSelectionChange = async (selectedRowKeys: string[] | number[]) => {
		this.setState({ selectedRowKeys });
	};

	// todo: do not copy this method from class to class - move to DataTable somehow?
	refreshTable = async (resetCurrentPage?: boolean, resetSelectedRows?: boolean) => {
		const { selectedRowKeys } = this.state;

		this.setState({
			updateTableToken: { date: new Date(), resetCurrentPage, resetSelectedRows },
			selectedRowKeys: resetSelectedRows ? [] : selectedRowKeys
		});
	};

	showAddPane = () => {
		this.setState({ showPane: true, editUid: null });
	};

	showEditPane = (data: Automation) => {
		this.setState({ showPane: true, editUid: data?.uid });
	};

	closePane = () => {
		this.setState({ showPane: false });
	};

	handleSuccess = () => {
		this.setState({ showPane: false });
		this.refreshTable(false);
	};

	delete = async () => {

		const t = (key: string) => i18next.t(key);

		await this._operation.execute(async () => {
			const { entityTypeCode, entityTypeUid } = this.props,
				{ selectedRowKeys } = this.state;

			const result = await this._automationService.delete({ entityTypeCode, entityTypeUid, uids: selectedRowKeys });

			if (result.success) {
				this.refreshTable(false, true);
			}

			return result;
		}, {
			showConfirm: true,
			confirmTitle: t("operation.confirm.delete.title")
		});
	};

	render = () => {
		const { entityTypeCode, entityTypeUid } = this.props,
			{ showPane, editUid, selectedRowKeys, updateTableToken } = this.state;

		return (<Translation>{(t) => <>

			<Toolbar clear>
				<ButtonAdd type="primary" onClick={this.showAddPane} />
				<ButtonDelete onClick={this.delete} disabled={!selectedRowKeys?.length} />
			</Toolbar>

			<DataTable
				rowKey="uid"
				rowActions={[{ name: t("button.edit"), onClick: this.showEditPane }]}
				viewId={`Automation/Grid`}
				loadUrl={`${Constants.apiURL}/automation/list/`}
				onLoadData={this.onLoadTableData}
				onSelectionChange={this.onSelectionChange}
				updateToken={updateTableToken}
				skipPaging={true}
			/>

			{showPane &&
				<PaneEditAutomation
					entityTypeCode={entityTypeCode}
					entityTypeUid={entityTypeUid}
					uid={editUid}
					onSuccess={this.handleSuccess}
					onClose={this.closePane}
				/>}

		</>}</Translation>);
	};
}
