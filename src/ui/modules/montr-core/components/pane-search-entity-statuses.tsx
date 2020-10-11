import React from "react";
import { WithTranslation, Translation, withTranslation } from "react-i18next";
import { Guid, IDataResult, IEntityStatus } from "../models";
import { OperationService, MetadataService } from "../services";
import { DataTableUpdateToken, Toolbar, ButtonAdd, ButtonDelete, DataTable, ModalEditEntityStatus } from ".";
import { Views } from "../module";
import { Constants } from "..";

interface Props extends WithTranslation {
	entityTypeCode: string;
	entityUid: Guid | string;
}

interface State {
	editData?: Partial<IEntityStatus>;
	showPane?: boolean;
	editUid?: Guid;
	selectedRowKeys?: string[] | number[];
	updateTableToken?: DataTableUpdateToken;
}

class WrappedPaneSearchEntityStatuses extends React.Component<Props, State> {

	private _operation = new OperationService();
	private _metadataService = new MetadataService();

	constructor(props: Props) {
		super(props);

		this.state = {
		};
	}

	componentDidMount = async () => {
	};

	componentWillUnmount = async () => {
		await this._metadataService.abort();
	};

	onLoadTableData = async (loadUrl: string, postParams: any): Promise<IDataResult<{}>> => {
		const { entityTypeCode, entityUid } = this.props;

		const params = { entityTypeCode, entityUid, ...postParams };

		return await this._metadataService.post(loadUrl, params);
	};

	onSelectionChange = async (selectedRowKeys: string[] | number[]) => {
		this.setState({ selectedRowKeys });
	};

	refreshTable = async (resetCurrentPage?: boolean, resetSelectedRows?: boolean) => {
		const { selectedRowKeys } = this.state;

		this.setState({
			updateTableToken: { date: new Date(), resetCurrentPage, resetSelectedRows },
			selectedRowKeys: resetSelectedRows ? [] : selectedRowKeys
		});
	};

	handleSuccess = () => {
		this.setState({ showPane: false });
		this.refreshTable(false);
	};

	showAddModal = () => {
		this.setState({ editData: {} });
	};

	delete = async () => {
		const { t } = this.props;

		await this._operation.execute(async () => {
			const { entityTypeCode, entityUid } = this.props,
				{ selectedRowKeys } = this.state;

			/* const result = await this._metadataService.delete({ entityTypeCode, entityUid, uids: selectedRowKeys });

			if (result.success) {
				this.refreshTable(false, true);
			}

			return result; */

			return null;
		}, {
			showConfirm: true,
			confirmTitle: t("operation.confirm.delete.title")
		});
	};

	onModalEditSuccess = async (data: IEntityStatus) => {
		this.setState({ editData: null });

		await this.refreshTable();
	};

	onModalEditCancel = () => {
		this.setState({ editData: null });
	};

	render = () => {
		const { entityTypeCode, entityUid } = this.props,
			{ editData, selectedRowKeys, updateTableToken } = this.state;

		return (<Translation>{(t) => <>

			<Toolbar clear>
				<ButtonAdd type="primary" onClick={this.showAddModal} />
				<ButtonDelete onClick={this.delete} disabled={!selectedRowKeys?.length} />
			</Toolbar>

			<DataTable
				rowKey="code"
				// rowActions={[{ name: t("button.edit"), onClick: this.showEditPane }]}
				viewId={Views.entityStatusList}
				loadUrl={`${Constants.apiURL}/entityStatus/list/`}
				onLoadData={this.onLoadTableData}
				onSelectionChange={this.onSelectionChange}
				updateToken={updateTableToken}
				skipPaging={true}
			/>


			{editData &&
				<ModalEditEntityStatus
					entityTypeCode={entityTypeCode}
					entityUid={entityUid}
					onSuccess={this.onModalEditSuccess}
					onCancel={this.onModalEditCancel}
				/>}

		</>}</Translation>);
	};
}

export const PaneSearchEntityStatuses = withTranslation()(WrappedPaneSearchEntityStatuses);
