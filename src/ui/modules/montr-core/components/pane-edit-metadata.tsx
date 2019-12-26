import * as React from "react";
import { Drawer, Modal } from "antd";
import { Toolbar } from "./toolbar";
import { IDataField, IDataResult, Guid, IMenu } from "../models";
import { MetadataService } from "../services";
import { DataTable, DataTableUpdateToken, ButtonAdd, PaneEditMetadataForm, ButtonDelete } from ".";
import { Constants } from "..";
import { Translation } from "react-i18next";

interface IProps {
	entityTypeCode: string;
}

interface IState {
	showDrawer?: boolean;
	editUid?: Guid;
	updateTableToken?: DataTableUpdateToken;
}

export class PaneEditMetadata extends React.Component<IProps, IState> {

	private _metadataService = new MetadataService();

	constructor(props: IProps) {
		super(props);

		this.state = {
		};

		const fields: IDataField[] = [
			{ key: "fullName", name: "Полное наименование", type: "string" },
			{ key: "shortName", name: "Сокращенное наименование", type: "string" },
			{ key: "address", name: "Адрес в пределах места пребывания", type: "address" },
			{ key: "okved", name: "Код(ы) ОКВЭД", type: "classifier" },
			{ key: "inn", name: "ИНН", type: "string" },
			{ key: "kpp", name: "КПП", type: "string" },
			{ key: "dp", name: "Дата постановки на учет в налоговом органе", type: "date" },
			{ key: "ogrn", name: "ОГРН", type: "string" },
			{ key: "is_msp", name: "Участник закупки является субъектом малого предпринимательства", type: "boolean" },
		];
	}

	componentDidMount = async () => {
	};

	componentWillUnmount = async () => {
		await this._metadataService.abort();
	};

	// todo: do not copy this method from class to class - move to DataTable somehow?
	refreshTable = async (resetSelectedRows?: boolean) => {
		this.setState({
			updateTableToken: { date: new Date(), resetSelectedRows }
		});
	};

	onLoadTableData = async (loadUrl: string, postParams: any): Promise<IDataResult<{}>> => {
		const { entityTypeCode } = this.props;

		const params = { entityTypeCode, ...postParams };

		return await this._metadataService.post(loadUrl, params);
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
		this.refreshTable();
	};

	showDeleteConfirm = (data: IDataField) => {
		Modal.confirm({
			title: "Вы действительно хотите удалить выбранное поле?",
			content: "Наверняка что-то случится ...",
			onOk: async () => {
				const { entityTypeCode } = this.props;

				await this._metadataService.delete(entityTypeCode, [data.uid]);

				this.refreshTable();
			}
		});
	};

	render = () => {
		const { entityTypeCode } = this.props,
			{ showDrawer, editUid, updateTableToken } = this.state;

		return (
			<Translation>{(t) => <>
				<Toolbar clear>
					<ButtonAdd type="primary" onClick={this.showAddDrawer} />
					<ButtonDelete />
				</Toolbar>

				<DataTable
					rowKey="key"
					rowActions={[{ name: t("button.edit"), onClick: this.showEditDrawer }]}
					viewId={`Metadata/Grid`}
					loadUrl={`${Constants.apiURL}/metadata/list/`}
					onLoadData={this.onLoadTableData}
					updateToken={updateTableToken}
				/>

				{showDrawer &&
					<Drawer
						title="Metadata"
						closable={false}
						onClose={this.closeDrawer}
						visible={true}
						width={800}>
						<PaneEditMetadataForm
							entityTypeCode={entityTypeCode} uid={editUid}
							onSuccess={this.handleSuccess}
						/>
					</Drawer>}
			</>}</Translation>);
	};
}
