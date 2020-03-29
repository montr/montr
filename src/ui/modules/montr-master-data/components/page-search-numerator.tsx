import React from "react";
import { PageHeader, Page, DataTable } from "@montr-core/components";
import { Views, Api } from "../module";
import { Translation } from "react-i18next";

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
