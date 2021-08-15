import * as React from "react";
import { Translation, WithTranslation, withTranslation } from "react-i18next";
import { ButtonAdd, ButtonDelete, DataTable, DataTableUpdateToken, PaneEditMetadataItem } from ".";
import { DataResult, Guid, IDataField } from "../models";
import { Api, Views } from "../module";
import { MetadataService, OperationService } from "../services";
import { Toolbar } from "./toolbar";

interface Props extends WithTranslation {
	mode: "fields" | "form";
	entityTypeCode: string;
	entityUid: Guid;
}

interface State {
	showPane?: boolean;
	editUid?: Guid;
	selectedRowKeys?: string[] | number[];
	updateTableToken?: DataTableUpdateToken;
}

class WrappedPaneEditMetadata extends React.Component<Props, State> {

	private operation = new OperationService();
	private metadataService = new MetadataService();

	constructor(props: Props) {
		super(props);

		this.state = {
		};
	}

	componentWillUnmount = async () => {
		await this.metadataService.abort();
	};

	onLoadTableData = async (loadUrl: string, postParams: any): Promise<DataResult<{}>> => {
		const { entityTypeCode, entityUid } = this.props;

		const params = { entityTypeCode, entityUid, ...postParams };

		return await this.metadataService.post(loadUrl, params);
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
		await this.operation.confirmDelete(async () => {
			const { entityTypeCode, entityUid } = this.props,
				{ selectedRowKeys } = this.state;

			const result = await this.metadataService.delete({ entityTypeCode, entityUid, uids: selectedRowKeys });

			if (result.success) {
				this.refreshTable(false, true);
			}

			return result;
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
				<PaneEditMetadataItem
					entityTypeCode={entityTypeCode}
					entityUid={entityUid}
					uid={editUid}
					onSuccess={this.handleSuccess}
					onClose={this.closePane}
				/>}

		</>}</Translation>);
	};
}

export const PaneEditMetadata = withTranslation()(WrappedPaneEditMetadata);
