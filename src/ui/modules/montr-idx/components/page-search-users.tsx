import React from "react";
import { Link } from "react-router-dom";
import { Button } from "antd";
import { Page, Toolbar, DataBreadcrumb, PageHeader, DataTable } from "@montr-core/components";
import { Constants } from "@montr-core/.";

interface IProps {
}

interface IState {
}

export default class SearchUsers extends React.Component<IProps, IState> {
	render = () => {
		return (
			<Page title={<>
				<Toolbar float="right">
					<Link to="/events/new"><Button icon="plus">Создать</Button></Link>
				</Toolbar>

				<DataBreadcrumb items={[]} />
				<PageHeader>Пользователи</PageHeader>
			</>}>

				<DataTable
					rowKey="uid"
					viewId="UserSearch/Grid"
					loadUrl={`${Constants.apiURL}/User/List`} />
			</Page>
		);
	};
};
