import React from "react";
import { PageHeader, Page, DataTable, Toolbar, ButtonAdd } from "@montr-core/components";
import { Views, Api, RouteBuilder } from "../module";
import { Translation } from "react-i18next";
import { Link } from "react-router-dom";

interface IProps {
}

interface IState {
}

export default class PageSearchNumerator extends React.Component<IProps, IState> {
	render = () => {
		return (
			<Translation ns="master-data">
				{(t) => <Page
					title={<>

						<Toolbar float="right">
							<Link to={RouteBuilder.addNumerator()}>
								<ButtonAdd type="primary" />
							</Link>
						</Toolbar>

						<PageHeader>{t("page.searchNumerators.title")}</PageHeader>
					</>}>

					<DataTable
						rowKey="uid"
						viewId={Views.numeratorList}
						loadUrl={Api.numeratorList}
					/>

				</Page>}
			</Translation>
		);
	};
}
