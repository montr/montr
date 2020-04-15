import React from "react";
import { OperationService } from "@montr-core/services";
import { DataTableUpdateToken, DataTable } from "@montr-core/components";
import { Views, Api } from "../module";

interface IProps {
}

interface IState {
	selectedRowKeys: string[] | number[];
	updateTableToken: DataTableUpdateToken;
}

export class TabEditNumeratorEntities extends React.Component<IProps, IState> {

	private _operation = new OperationService();

	constructor(props: IProps) {
		super(props);

		this.state = {
			selectedRowKeys: [],
			updateTableToken: { date: new Date() }
		};
	}

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
				updateToken={updateTableToken}
			/>
		);
	};
}
