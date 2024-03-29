import { DataTable, DataTableUpdateToken } from "@montr-core/components/data-table";
import { DataResult } from "@montr-core/models";
import { Fetcher } from "@montr-core/services";
import React from "react";
import { Numerator } from "../models";
import { Api, Views } from "../module";

interface Props {
	data: Numerator;
}

interface State {
	selectedRowKeys: string[] | number[];
	updateTableToken: DataTableUpdateToken;
}

export default class TabEditNumeratorEntities extends React.Component<Props, State> {

	private _numeratorService = new Fetcher();

	constructor(props: Props) {
		super(props);

		this.state = {
			selectedRowKeys: [],
			updateTableToken: { date: new Date() }
		};
	}

	componentWillUnmount = async (): Promise<void> => {
		await this._numeratorService.abort();
	};

	onLoadTableData = async (loadUrl: string, postParams: any): Promise<DataResult<unknown>> => {
		const { data } = this.props;

		const params = { numeratorUid: data.uid, ...postParams };

		return await this._numeratorService.post(loadUrl, params);
	};

	refreshTable = async (resetSelectedRows?: boolean): Promise<void> => {
		const { selectedRowKeys } = this.state;

		this.setState({
			updateTableToken: { date: new Date(), resetSelectedRows },
			selectedRowKeys: resetSelectedRows ? [] : selectedRowKeys
		});
	};

	render = (): React.ReactNode => {
		const { updateTableToken } = this.state;

		return (
			<DataTable
				rowKey="uid"
				viewId={Views.numeratorEntityList}
				loadUrl={Api.numeratorEntityList}
				onLoadData={this.onLoadTableData}
				updateToken={updateTableToken}
			/>
		);
	};
}
