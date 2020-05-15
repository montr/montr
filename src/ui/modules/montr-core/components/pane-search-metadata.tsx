import * as React from "react";
import { Drawer } from "antd";
import { Toolbar } from "./toolbar";
import { IDataField, IDataResult, Guid } from "../models";
import { MetadataService, OperationService } from "../services";
import { DataTable, DataTableUpdateToken, ButtonAdd, PaneEditMetadata, ButtonDelete } from ".";
import { Constants } from "..";
import { Translation, WithTranslation, withTranslation } from "react-i18next";

interface IProps extends WithTranslation {
	entityTypeCode: string;
	entityUid: Guid | string;
}

interface IState {
	showDrawer?: boolean;
	editUid?: Guid;
	selectedRowKeys?: string[] | number[];
	updateTableToken?: DataTableUpdateToken;
}

class WrappedPaneSearchMetadata extends React.Component<IProps, IState> {

	private _operation = new OperationService();
	private _metadataService = new MetadataService();

	constructor(props: IProps) {
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

	// todo: do not copy this method from class to class - move to DataTable somehow?
	refreshTable = async (resetCurrentPage?: boolean, resetSelectedRows?: boolean) => {
		const { selectedRowKeys } = this.state;

		this.setState({
			updateTableToken: { date: new Date(), resetCurrentPage, resetSelectedRows },
			selectedRowKeys: resetSelectedRows ? [] : selectedRowKeys
		});
	};

	showAddDrawer = () => {
		this.setState({ showDrawer: true, editUid: null });
	};

	showEditDrawer = (data: IDataField) => {
		this.setState({ showDrawer: true, editUid: data?.uid });
	};

	closeDrawer = () => {
		this.setState({ showDrawer: false });
	};

	handleSuccess = () => {
		this.setState({ showDrawer: false });
		this.refreshTable(false);
	};

	delete = async () => {
		const { t } = this.props;

		await this._operation.execute(async () => {
			const { entityTypeCode, entityUid } = this.props,
				{ selectedRowKeys } = this.state;

			const result = await this._metadataService.delete({ entityTypeCode, entityUid, uids: selectedRowKeys });

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
		const { entityTypeCode, entityUid } = this.props,
			{ showDrawer, editUid, selectedRowKeys, updateTableToken } = this.state;

		return (<Translation>{(t) => <>

			<Toolbar clear>
				<ButtonAdd type="primary" onClick={this.showAddDrawer} />
				<ButtonDelete onClick={this.delete} disabled={!selectedRowKeys?.length} />
			</Toolbar>

			<DataTable
				rowKey="uid"
				rowActions={[{ name: t("button.edit"), onClick: this.showEditDrawer }]}
				viewId={`Metadata/Grid`}
				loadUrl={`${Constants.apiURL}/metadata/list/`}
				onLoadData={this.onLoadTableData}
				onSelectionChange={this.onSelectionChange}
				updateToken={updateTableToken}
				skipPaging={true}
			/>

			{showDrawer &&
				// todo: move drawer to pane-edit-metadata (?)
				<Drawer
					title="Metadata"
					closable={true}
					onClose={this.closeDrawer}
					visible={true}
					width={720}
					bodyStyle={{ paddingBottom: "80px" }}>
					<PaneEditMetadata
						entityTypeCode={entityTypeCode}
						entityUid={entityUid}
						uid={editUid}
						onSuccess={this.handleSuccess}
					/>
				</Drawer>}

		</>}</Translation>);
	};
}

export const PaneSearchMetadata = withTranslation()(WrappedPaneSearchMetadata);
