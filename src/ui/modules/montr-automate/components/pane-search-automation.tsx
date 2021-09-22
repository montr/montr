import { ButtonAdd, ButtonDelete, DataTable, DataTableUpdateToken, Toolbar } from "@montr-core/components";
import { DataPaneProps, DataResult, Guid } from "@montr-core/models";
import { OperationService } from "@montr-core/services";
import React from "react";
import { Translation } from "react-i18next";
import { Automation } from "../models/automation";
import { Api, Views } from "../module";
import { AutomationService } from "../services/automation-service";
import { PaneEditAutomation } from "./pane-edit-automation";

interface Props extends DataPaneProps<unknown> {
}

interface State {
	showPane?: boolean;
	editUid?: Guid;
	selectedRowKeys?: string[] | number[];
	updateTableToken?: DataTableUpdateToken;
}

export default class PaneSearchAutomation extends React.Component<Props, State> {

	operation = new OperationService();
	automationService = new AutomationService();

	constructor(props: Props) {
		super(props);

		this.state = {
		};
	}

	componentWillUnmount = async (): Promise<void> => {
		await this.automationService.abort();
	};

	onLoadTableData = async (loadUrl: string, postParams: any): Promise<DataResult<{}>> => {
		const { entityTypeCode, entityUid } = this.props;

		const params = { entityTypeCode, entityUid, ...postParams };

		return await this.automationService.post(loadUrl, params);
	};

	onSelectionChange = async (selectedRowKeys: string[] | number[]): Promise<void> => {
		this.setState({ selectedRowKeys });
	};

	refreshTable = async (resetCurrentPage?: boolean, resetSelectedRows?: boolean): Promise<void> => {
		const { selectedRowKeys } = this.state;

		this.setState({
			updateTableToken: { date: new Date(), resetCurrentPage, resetSelectedRows },
			selectedRowKeys: resetSelectedRows ? [] : selectedRowKeys
		});
	};

	showAddPane = (): void => {
		this.setState({ showPane: true, editUid: null });
	};

	showEditPane = (data: Automation): void => {
		this.setState({ showPane: true, editUid: data?.uid });
	};

	closePane = (): void => {
		this.setState({ showPane: false });
	};

	handleSuccess = (): void => {
		this.setState({ showPane: false });
		this.refreshTable(false);
	};

	delete = async (): Promise<void> => {
		await this.operation.confirmDelete(async () => {
			const { entityTypeCode, entityUid } = this.props,
				{ selectedRowKeys } = this.state;

			const result = await this.automationService.delete({ entityTypeCode, entityUid, uids: selectedRowKeys });

			if (result.success) {
				this.refreshTable(false, true);
			}

			return result;
		});
	};

	render = (): React.ReactNode => {
		const { entityTypeCode, entityUid } = this.props,
			{ showPane, editUid, selectedRowKeys, updateTableToken } = this.state;

		return (<Translation>{(t) => <>

			<Toolbar clear>
				<ButtonAdd type="primary" onClick={this.showAddPane} />
				<ButtonDelete onClick={this.delete} disabled={!selectedRowKeys?.length} />
			</Toolbar>

			<DataTable
				rowKey="uid"
				rowActions={[{ name: t("button.edit"), onClick: this.showEditPane }]}
				viewId={Views.automationList}
				loadUrl={Api.automationList}
				onLoadData={this.onLoadTableData}
				onSelectionChange={this.onSelectionChange}
				updateToken={updateTableToken}
				skipPaging={true}
			/>

			{showPane &&
				<PaneEditAutomation
					entityTypeCode={entityTypeCode}
					entityUid={entityUid}
					uid={editUid}
					onSuccess={this.handleSuccess}
					onClose={this.closePane}
				/>}

		</>}</Translation>);
	};
}
