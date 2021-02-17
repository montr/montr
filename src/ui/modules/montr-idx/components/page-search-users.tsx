import React from "react";
import { Link } from "react-router-dom";
import { Page, Toolbar, DataBreadcrumb, PageHeader, DataTable, ButtonAdd } from "@montr-core/components";
import { Api, Views } from "../module";

interface Props {
}

interface State {
}

export default class PageSearchUsers extends React.Component<Props, State> {
	render = () => {
		return (
			<Page title={<>
				<Toolbar float="right">
					<Link to="/users/new"><ButtonAdd type="primary" /></Link>
				</Toolbar>

				<DataBreadcrumb items={[{ name: "Пользователи" }]} />
				<PageHeader>Пользователи</PageHeader>
			</>}>

				<DataTable
					rowKey="uid"
					viewId={Views.gridSearchUsers}
					loadUrl={Api.userList} />
			</Page>
		);
	};
};
