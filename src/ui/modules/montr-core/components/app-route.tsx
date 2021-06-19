import React from "react";
import { Route, RouteComponentProps } from "react-router";
import { IRoute } from "../models";

export const AppRoute = ({ component: Component, layout: Layout, layoutComponent: LayoutComponent, ...rest }: IRoute): React.ReactElement => (
	<Route {...rest} render={(props: RouteComponentProps<unknown>) => (
		<LayoutComponent>
			<Component {...props} />
		</LayoutComponent>
	)} />
);
