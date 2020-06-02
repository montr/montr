import React from "react";
import { withTranslation, WithTranslation, Translation } from "react-i18next";
import { DataTableUpdateToken, Toolbar, ButtonAdd, ButtonDelete, DataTable } from ".";
import { Guid, IDataResult } from "../models";
import { OperationService, Fetcher } from "../services";
import { Constants } from "..";

interface IProps extends WithTranslation {
	entityTypeCode: string;
	entityTypeUid: Guid | string;
}

interface IState {
	showDrawer?: boolean;
	editUid?: Guid;
	selectedRowKeys?: string[] | number[];
	updateTableToken?: DataTableUpdateToken;
}

class WrappedPaneSearchAutomation extends React.Component<IProps, IState> {

	private _operation = new OperationService();
	private _fetcher = new Fetcher();

	constructor(props: IProps) {
		super(props);

		this.state = {
		};
	}

	componentDidMount = async () => {
	};

	componentWillUnmount = async () => {
		await this._fetcher.abort();
	};

	onLoadTableData = async (loadUrl: string, postParams: any): Promise<IDataResult<{}>> => {
		const { entityTypeCode, entityTypeUid } = this.props;

		const params = { entityTypeCode, entityTypeUid, ...postParams };

		return await this._fetcher.post(loadUrl, params);
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

	render = () => {
		const { entityTypeCode, entityTypeUid } = this.props,
			{ showDrawer, editUid, selectedRowKeys, updateTableToken } = this.state;

		return (<Translation>{(t) => <>

			<Toolbar clear>
				<ButtonAdd type="primary" /* onClick={this.showAddDrawer} */ />
				<ButtonDelete /* onClick={this.delete} */ disabled={!selectedRowKeys?.length} />
			</Toolbar>

			<DataTable
				rowKey="uid"
				rowActions={[{ name: t("button.edit")/* , onClick: this.showEditDrawer */ }]}
				viewId={`Automation/Grid`}
				loadUrl={`${Constants.apiURL}/automation/list/`}
				onLoadData={this.onLoadTableData}
				onSelectionChange={this.onSelectionChange}
				updateToken={updateTableToken}
				skipPaging={true}
			/>

			{/* {showDrawer &&
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
				</Drawer>} */}

		</>}</Translation>);
	};
}

export const PaneSearchAutomation = withTranslation()(WrappedPaneSearchAutomation);
