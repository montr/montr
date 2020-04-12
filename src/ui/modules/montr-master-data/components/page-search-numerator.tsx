import React from "react";
import { PageHeader, Page, DataTable, Toolbar, ButtonAdd, ButtonDelete, DataTableUpdateToken, DataBreadcrumb } from "@montr-core/components";
import { Views, Api, RouteBuilder, Patterns } from "../module";
import { Translation } from "react-i18next";
import { Link, Route } from "react-router-dom";
import { NumeratorService } from "../services";
import { OperationService } from "@montr-core/services";

interface IProps {
}

interface IState {
	selectedRowKeys: string[] | number[];
	updateTableToken: DataTableUpdateToken;
}

export default class PageSearchNumerator extends React.Component<IProps, IState> {

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

	onSelectionChange = async (selectedRowKeys: string[] | number[]) => {
		this.setState({ selectedRowKeys });
	};

	// todo: show confirm
	delete = async () => {

		const { selectedRowKeys } = this.state;

		const result = await this._operation.execute(() =>
			this._numeratorService.delete(selectedRowKeys)
		);

		if (result.success) {
			this.refreshTable(true);
		}
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
			<Translation ns="master-data">
				{(t) => <Page
					title={<>

						<Toolbar float="right">
							<Link to={RouteBuilder.addNumerator()}>
								<ButtonAdd type="primary" />
							</Link>
							<ButtonDelete onClick={this.delete} disabled={selectedRowKeys.length == 0} />
						</Toolbar>

						<DataBreadcrumb items={[
							{ name: t("page.searchNumerators.title"), route: Patterns.searchNumerator }
						]} />

						<PageHeader>{t("page.searchNumerators.title")}</PageHeader>
					</>}>

					<DataTable
						rowKey="uid"
						viewId={Views.numeratorList}
						loadUrl={Api.numeratorList}
						onSelectionChange={this.onSelectionChange}
						updateToken={updateTableToken}
					/>

				</Page>}
			</Translation>
		);
	};
}
