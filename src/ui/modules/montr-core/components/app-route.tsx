import React from "react";
import { Route, RouteComponentProps } from "react-router";

interface IProps {
	component: React.ComponentType<RouteComponentProps<any>> | React.ComponentType<any>;
	layout: any;
}

export const AppRoute = ({ component: Component, layout: Layout, ...rest }: IProps) => (
	<Route {...rest} render={props => (
		<Layout>
			<Component {...props} />
		</Layout>
	)} />
)

