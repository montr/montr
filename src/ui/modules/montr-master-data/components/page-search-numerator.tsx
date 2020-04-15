import React from "react";
import { Link } from "react-router-dom";
import { Translation, WithTranslation, withTranslation } from "react-i18next";
import { PageHeader, Page, DataTable, Toolbar, ButtonAdd, ButtonDelete, DataTableUpdateToken, DataBreadcrumb } from "@montr-core/components";
import { OperationService } from "@montr-core/services";
import { Views, Api, RouteBuilder, Patterns, Locale } from "../module";
import { NumeratorService } from "../services";

interface IProps extends WithTranslation {
}

interface IState {
	selectedRowKeys: string[] | number[];
	updateTableToken: DataTableUpdateToken;
}

class WrappedPageSearchNumerator extends React.Component<IProps, IState> {

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

	delete = async () => {
		const { t } = this.props;

		await this._operation.execute(async () => {
			const { selectedRowKeys } = this.state;

			const result = await this._numeratorService.delete(selectedRowKeys);
			if (result.success) {
				this.refreshTable(true);
			}
			return result;
		}, {
			showConfirm: true,
			confirmTitle: t("operation.confirm.delete.title")
		});
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
			<Translation ns={Locale.Namespace}>
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

const PageSearchNumerator = withTranslation(Locale.Namespace)(WrappedPageSearchNumerator);

export default PageSearchNumerator;
