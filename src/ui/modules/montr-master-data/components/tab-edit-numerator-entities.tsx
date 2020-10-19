import React from "react";
import { OperationService } from "@montr-core/services";
import { DataTableUpdateToken, DataTable } from "@montr-core/components";
import { Views, Api } from "../module";
import { DataResult } from "@montr-core/models";
import { INumerator } from "@montr-master-data/models";
import { NumeratorService } from "@montr-master-data/services";

interface IProps {
	data: INumerator;
}

interface IState {
	selectedRowKeys: string[] | number[];
	updateTableToken: DataTableUpdateToken;
}

export class TabEditNumeratorEntities extends React.Component<IProps, IState> {

	private _operation = new OperationService();
	private _numeratorService = new NumeratorService();

	constructor(props: IProps) {
		super(props);

		this.state = {
			selectedRowKeys: [],
			updateTableToken: { date: new Date() }
		};
	}

	componentWillUnmount = async () => {
		await this._numeratorService.abort();
	};

	onLoadTableData = async (loadUrl: string, postParams: any): Promise<DataResult<{}>> => {
		const { data } = this.props;

		const params = { numeratorUid: data.uid, ...postParams };

		return await this._numeratorService.post(loadUrl, params);
	};

	refreshTable = async (resetSelectedRows?: boolean) => {
		const { selectedRowKeys } = this.state;

		this.setState({
			updateTableToken: { date: new Date(), resetSelectedRows },
			selectedRowKeys: resetSelectedRows ? [] : selectedRowKeys
		});
	};

	render = () => {
		const { selectedRowKeys, updateTableToken } = this.state;

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
