import React from "react";
import { Route } from "react-router-dom";
import { IRoute } from "../models";

export const AppRoute = ({ component: Component, layout: Layout, layoutComponent: LayoutComponent, ...rest }: IRoute): React.ReactElement => (
	<Route {...rest} render={(props: unknown) => (
		<LayoutComponent>
			<Component {...props} />
		</LayoutComponent>
	)} />
);
