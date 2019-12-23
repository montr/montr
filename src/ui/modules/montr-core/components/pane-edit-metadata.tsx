import * as React from "react";
import { Divider, Drawer } from "antd";
import { Toolbar } from "./toolbar";
import { IDataField, IDataResult } from "../models";
import { MetadataService } from "../services";
import { DataTable, DataTableUpdateToken, ButtonAdd, PaneEditMetadataForm } from ".";
import { Constants } from "..";

interface IProps {
	entityTypeCode: string;
}

interface IState {
	showDrawer?: boolean;
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

	refreshTable = async () => {

		this.setState({
			updateTableToken: { date: new Date() }
		});
	};

	onLoadTableData = async (loadUrl: string, postParams: any): Promise<IDataResult<{}>> => {
		const { entityTypeCode } = this.props;

		const params = { entityTypeCode, ...postParams };

		return await this._metadataService.post(loadUrl, params);
	};

	showAddDrawer = () => {
		this.setState({ showDrawer: true });
	};

	onCloseDrawer = () => {
		this.setState({ showDrawer: false });
	};

	render = () => {
		const { entityTypeCode } = this.props,
			{ showDrawer, updateTableToken } = this.state;

		return (<>
			<Toolbar>
				<ButtonAdd onClick={this.showAddDrawer} />
			</Toolbar>

			<Divider />

			<DataTable
				rowKey="key"
				// rowActions={rowActions}
				viewId={`Metadata/Grid`}
				loadUrl={`${Constants.apiURL}/metadata/list/`}
				onLoadData={this.onLoadTableData}
				updateToken={updateTableToken}
			/>

			{showDrawer &&
				<Drawer
					title="Metadata"
					closable={false}
					onClose={this.onCloseDrawer}
					visible={true}
					width={800}>
					<PaneEditMetadataForm
						entityTypeCode={entityTypeCode}
					/>
				</Drawer>}
		</>);
	};
}
