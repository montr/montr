import * as React from "react";
import { Page, DataTable } from "@montr-core/components";
import { RouteComponentProps } from "react-router";
import { Constants } from "@montr-core/.";
import { Icon, Button } from "antd";
import { Link } from "react-router-dom";

interface IRouteProps {
	configCode: string;
}

interface IProps {
}

export const SearchClassifier = (props: IProps & RouteComponentProps<IRouteProps>) => {

	return (
		<Page title={props.match.params.configCode} toolbar={
			<Link to={`/classifiers/${props.match.params.configCode}/new`}>
				<Button type="primary"><Icon type="plus" /> Добавить элемент</Button>
			</Link>
		}>

			<DataTable
				viewId="ClassifierList/Grid"
				loadUrl={`${Constants.baseURL}/classifier/list/?configCode=${props.match.params.configCode}`}
			/>

		</Page>
	);
}
