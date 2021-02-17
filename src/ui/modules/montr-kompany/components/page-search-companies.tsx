import React from "react";
import { Link } from "react-router-dom";
import { Page, Toolbar, DataBreadcrumb, PageHeader, DataTable, ButtonAdd } from "@montr-core/components";
import { Api, Views } from "../module";

interface Props {
}

interface State {
}

export default class PageSearchCompanies extends React.Component<Props, State> {
	render = () => {
		return (
			<Page title={<>
				<Toolbar float="right">
					<Link to="/companies/new"><ButtonAdd type="primary" /></Link>
				</Toolbar>

				<DataBreadcrumb items={[{ name: "Компании" }]} />
				<PageHeader>Компании</PageHeader>
			</>}>

				<DataTable
					rowKey="uid"
					viewId={Views.gridSearchCompanies}
					loadUrl={Api.companyList} />
			</Page>
		);
	};
};
