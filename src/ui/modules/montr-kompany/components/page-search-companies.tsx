import React from "react";
import { Link } from "react-router-dom";
import { Page, Toolbar, DataBreadcrumb, PageHeader, DataTable, ButtonAdd } from "@montr-core/components";
import { Api, Views } from "../module";

interface IProps {
}

interface IState {
}

export default class PageSearchCompanies extends React.Component<IProps, IState> {
	render = () => {
		return (
			<Page title={<>
				<Toolbar float="right">
					<Link to="/companies/new"><ButtonAdd /></Link>
				</Toolbar>

				<DataBreadcrumb items={[]} />
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
