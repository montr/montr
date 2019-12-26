import React from "react";
import { Link } from "react-router-dom";
import { Page, Toolbar, DataBreadcrumb, PageHeader, DataTable, ButtonAdd } from "@montr-core/components";
import { Api } from "../module";

interface IProps {
}

interface IState {
}

export default class SearchUsers extends React.Component<IProps, IState> {
	render = () => {
		return (
			<Page title={<>
				<Toolbar float="right">
					<Link to="/users/new"><ButtonAdd /></Link>
				</Toolbar>

				<DataBreadcrumb items={[]} />
				<PageHeader>Пользователи</PageHeader>
			</>}>

				<DataTable
					rowKey="uid"
					viewId="UserSearch/Grid"
					loadUrl={Api.userList} />
			</Page>
		);
	};
};
