import React from "react";
import { Translation, WithTranslation, withTranslation } from "react-i18next";
import { ButtonAdd, ButtonDelete, DataTable, DataTableUpdateToken, ModalEditEntityStatus, Toolbar } from ".";
import { DataResult, EntityStatus, Guid } from "../models";
import { Api, Views } from "../module";
import { EntityStatusService, MetadataService, OperationService } from "../services";

interface Props extends WithTranslation {
	entityTypeCode: string;
	entityUid: Guid;
}

interface State {
	editData?: Partial<EntityStatus>;
	showPane?: boolean;
	editUid?: Guid;
	selectedRowKeys?: string[] | number[];
	updateTableToken?: DataTableUpdateToken;
}

class WrappedPaneSearchEntityStatuses extends React.Component<Props, State> {

	private readonly operation = new OperationService();
	private readonly metadataService = new MetadataService();
	private readonly entityStatusService = new EntityStatusService();

	constructor(props: Props) {
		super(props);

		this.state = {
		};
	}

	componentWillUnmount = async () => {
		await this.metadataService.abort();
		await this.entityStatusService.abort();
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

	handleSuccess = () => {
		this.setState({ showPane: false });
		this.refreshTable(false);
	};

	showAddModal = () => {
		this.setState({ editData: {} });
	};

	showEditModal = (data: EntityStatus) => {
		this.setState({ editData: data });
	};

	delete = async () => {
		await this.operation.confirmDelete(async () => {
			const { entityTypeCode, entityUid } = this.props,
				{ selectedRowKeys } = this.state;

			const result = await this.entityStatusService.delete({ entityTypeCode, entityUid, uids: selectedRowKeys });

			if (result.success) {
				this.refreshTable(false, true);
			}

			return result;
		});
	};

	onModalEditSuccess = async (data: EntityStatus) => {
		this.setState({ editData: null });

		await this.refreshTable();
	};

	onModalEditCancel = () => {
		this.setState({ editData: null });
	};

	render = (): React.ReactNode => {
		const { entityTypeCode, entityUid } = this.props,
			{ editData, selectedRowKeys, updateTableToken } = this.state;

		return (<Translation>{(t) => <>

			<Toolbar clear>
				<ButtonAdd type="primary" onClick={this.showAddModal} />
				<ButtonDelete onClick={this.delete} disabled={!selectedRowKeys?.length} />
			</Toolbar>

			<DataTable
				rowKey="uid"
				rowActions={[{ name: t("button.edit"), onClick: this.showEditModal }]}
				viewId={Views.entityStatusList}
				loadUrl={Api.entityStatusList}
				onLoadData={this.onLoadTableData}
				onSelectionChange={this.onSelectionChange}
				updateToken={updateTableToken}
				skipPaging={true}
			/>

			{editData &&
				<ModalEditEntityStatus
					entityTypeCode={entityTypeCode}
					entityUid={entityUid}
					uid={editData.uid}
					onSuccess={this.onModalEditSuccess}
					onCancel={this.onModalEditCancel}
				/>}

		</>}</Translation>);
	};
}

const PaneSearchEntityStatuses = withTranslation()(WrappedPaneSearchEntityStatuses);

export default PaneSearchEntityStatuses;
