import React from "react";
import { Route, Routes } from "react-router";
import { BrowserRouter } from "react-router-dom";
import { IRoute } from "../models";
import { AppRoute, AppSetupRedirect } from "./";

interface Props {
	routes: IRoute[];
	layoutRegistry: (key: string) => React.ComponentType<unknown>;
	defaultLayout: string;
	errorLayout: string;
}

export const AppRouteList = ({ routes, layoutRegistry, defaultLayout, errorLayout }: Props): React.ReactElement => (
	<BrowserRouter>
		<AppSetupRedirect>
			<Routes>
				{routes.map(({ layout, ...props }, _do_not_remove_) => {
					return <AppRoute key={0} {...props} layoutComponent={layoutRegistry(layout || defaultLayout)} />;
				})}
				<AppRoute component={React.lazy(() => import("./page-error-404"))} layoutComponent={layoutRegistry(errorLayout)} />
			</Routes>
		</AppSetupRedirect>
	</BrowserRouter >
);

interface RouteListProps {
	routes: IRoute[];
}

export const RouteList = ({ routes }: RouteListProps): React.ReactElement => (
	<Routes>
		{routes.map(({ ...props }, _do_not_remove_) => {
			return <Route key={0} {...props} />;
		})}
		{/* <Route>
			<h1>404</h1>
		</Route> */}
	</Routes>
);
