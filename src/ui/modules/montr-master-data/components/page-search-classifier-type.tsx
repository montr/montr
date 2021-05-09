import * as React from "react";
import { Translation, withTranslation, WithTranslation } from "react-i18next";
import { Page, PageHeader, Toolbar, DataTable, DataTableUpdateToken, ButtonAdd, ButtonDelete } from "@montr-core/components";
import { Constants } from "@montr-core/.";
import { withCompanyContext, CompanyContextProps } from "@montr-kompany/components";
import { ClassifierBreadcrumb } from ".";
import { Link } from "react-router-dom";
import { ClassifierTypeService } from "../services";
import { OperationService } from "@montr-core/services";
import { IMenu } from "@montr-core/models";
import { ClassifierGroup } from "../models";
import { RouteBuilder, Locale } from "../module";

interface Props extends CompanyContextProps, WithTranslation {
}

interface State {
	selectedRowKeys: string[] | number[];
	updateTableToken: DataTableUpdateToken;
}

class WrappedSearchClassifierType extends React.Component<Props, State> {

	private operation = new OperationService();
	private classifierTypeService = new ClassifierTypeService();

	constructor(props: Props) {
		super(props);

		this.state = {
			selectedRowKeys: [],
			updateTableToken: { date: new Date() }
		};
	}

	componentDidUpdate = async (prevProps: Props) => {
		// todo: detect company changed without CompanyContextProps (here and everywhere)
		if (this.props.currentCompany !== prevProps.currentCompany) {
			this.refreshTable(true);
		}
	};

	componentWillUnmount = async () => {
		await this.classifierTypeService.abort();
	};

	onSelectionChange = async (selectedRowKeys: string[] | number[]) => {
		this.setState({ selectedRowKeys });
	};

	delete = async () => {
		await this.operation.confirmDelete(async () => {
			const result = await this.classifierTypeService.delete(this.state.selectedRowKeys);

			if (result.success) {
				this.refreshTable(true);
			}

			return result;
		});
	};

	refreshTable = async (resetSelectedRows?: boolean) => {
		this.setState({
			updateTableToken: { date: new Date(), resetSelectedRows }
		});
	};

	render = (): React.ReactNode => {
		const { updateTableToken } = this.state;

		const rowActions: IMenu[] = [
			{ name: "Настроить", route: (item: ClassifierGroup) => RouteBuilder.editClassifierType(item.uid) }
		];

		return (
			<Translation ns={Locale.Namespace}>
				{(t) => <Page
					title={<>
						<Toolbar float="right">
							<Link to={`/classifiers/add`}>
								<ButtonAdd type="primary" />
							</Link>
							<ButtonDelete onClick={this.delete} />
						</Toolbar>

						<ClassifierBreadcrumb />
						<PageHeader>{t("page.searchClassifierTypes.title")}</PageHeader>
					</>}>

					<DataTable
						rowKey="uid"
						viewId={`ClassifierType/Grid/`}
						loadUrl={`${Constants.apiURL}/classifierType/list/`}
						onSelectionChange={this.onSelectionChange}
						updateToken={updateTableToken}
						rowActions={rowActions}
					/>

				</Page>}
			</Translation>
		);
	};
}

const SearchClassifierType = withTranslation()(withCompanyContext(WrappedSearchClassifierType));

export default SearchClassifierType;
