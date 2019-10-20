import React from "react";
import { Route, RouteComponentProps } from "react-router";
import { IRoute } from "../models";

export const AppRoute = ({ component: Component, layout: Layout, layoutComponent: LayoutComponent, ...rest }: IRoute) => (
	<Route {...rest} render={(props: RouteComponentProps<any>) => (
		<LayoutComponent>
			<Component {...props} />
		</LayoutComponent>
	)} />
)
