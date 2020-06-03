import React from "react";
import { withTranslation, WithTranslation, Translation } from "react-i18next";
import { DataTableUpdateToken, Toolbar, ButtonAdd, ButtonDelete, DataTable, PaneEditAutomation } from ".";
import { Guid, IDataResult, IAutomation } from "../models";
import { OperationService, AutomationService } from "../services";
import { Constants } from "..";

interface IProps extends WithTranslation {
	entityTypeCode: string;
	entityTypeUid: Guid | string;
}

interface IState {
	showPane?: boolean;
	editUid?: Guid;
	selectedRowKeys?: string[] | number[];
	updateTableToken?: DataTableUpdateToken;
}

class WrappedPaneSearchAutomation extends React.Component<IProps, IState> {

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

	showEditPane = (data: IAutomation) => {
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
		const { t } = this.props;

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

export const PaneSearchAutomation = withTranslation()(WrappedPaneSearchAutomation);
