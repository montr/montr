import * as React from "react";
import { Toolbar } from "./toolbar";
import { IDataField, DataResult, Guid } from "../models";
import { MetadataService, OperationService } from "../services";
import { DataTable, DataTableUpdateToken, ButtonAdd, PaneEditMetadata, ButtonDelete } from ".";
import { Translation, WithTranslation, withTranslation } from "react-i18next";
import { Api, Views } from "../module";

interface Props extends WithTranslation {
	entityTypeCode: string;
	entityUid: Guid;
}

interface State {
	showPane?: boolean;
	editUid?: Guid;
	selectedRowKeys?: string[] | number[];
	updateTableToken?: DataTableUpdateToken;
}

class WrappedPaneSearchMetadata extends React.Component<Props, State> {

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

	onLoadTableData = async (loadUrl: string, postParams: any): Promise<DataResult<{}>> => {
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

	showAddPane = () => {
		this.setState({ showPane: true, editUid: null });
	};

	showEditPane = (data: IDataField) => {
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
			{ showPane, editUid, selectedRowKeys, updateTableToken } = this.state;

		return (<Translation>{(t) => <>

			<Toolbar clear>
				<ButtonAdd type="primary" onClick={this.showAddPane} />
				<ButtonDelete onClick={this.delete} disabled={!selectedRowKeys?.length} />
			</Toolbar>

			<DataTable
				rowKey="uid"
				rowActions={[{ name: t("button.edit"), onClick: this.showEditPane }]}
				viewId={Views.metadataList}
				loadUrl={Api.metadataList}
				onLoadData={this.onLoadTableData}
				onSelectionChange={this.onSelectionChange}
				updateToken={updateTableToken}
				skipPaging={true}
			/>

			{showPane &&
				<PaneEditMetadata
					entityTypeCode={entityTypeCode}
					entityUid={entityUid}
					uid={editUid}
					onSuccess={this.handleSuccess}
					onClose={this.closePane}
				/>}

		</>}</Translation>);
	};
}

const PaneSearchMetadata = withTranslation()(WrappedPaneSearchMetadata);

export default PaneSearchMetadata;
