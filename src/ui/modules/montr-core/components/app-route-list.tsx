import React from "react";
import { Switch, Route } from "react-router";
import { BrowserRouter } from "react-router-dom";
import { AppRoute, AppSetupRedirect } from "./";
import { IRoute } from "../models";

interface Props {
	routes: IRoute[];
	layoutRegistry: (key: string) => React.ComponentType<any>;
	defaultLayout: string;
	errorLayout: string;
}

export const AppRouteList = ({ routes, layoutRegistry, defaultLayout, errorLayout }: Props) => (
	<BrowserRouter>
		<AppSetupRedirect>
			<Switch>
				{routes.map(({ layout, ...props }, _do_not_use_) => {
					return <AppRoute key={0} {...props} layoutComponent={layoutRegistry(layout || defaultLayout)} />;
				})}
				<AppRoute component={React.lazy(() => import("./page-error-404"))} layoutComponent={layoutRegistry(errorLayout)} />
			</Switch>
		</AppSetupRedirect>
	</BrowserRouter >
);

interface RouteListProps {
	routes: IRoute[];
}

export const RouteList = ({ routes }: RouteListProps) => (
	<Switch>
		{routes.map(({ ...props }, _do_not_use_) => {
			return <Route key={0} {...props} />;
		})}
		<Route>
			<h1>404</h1>
		</Route>
	</Switch>
);
