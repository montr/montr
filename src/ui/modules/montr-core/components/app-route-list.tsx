import React from "react";
import { BrowserRouter, Route, Routes } from "react-router-dom";
import { IRoute } from "../models";
import { AppSetupRedirect } from "./";

interface Props {
	routes: IRoute[];
	layoutRegistry: (key: string) => React.ComponentType<unknown>;
	defaultLayout: string;
	errorLayout: string;
}

const PageError404 = React.lazy(() => import("./page-error-404"));

export const AppRouteList = ({ routes, layoutRegistry, defaultLayout, errorLayout }: Props): React.ReactElement => (
	<BrowserRouter>
		<AppSetupRedirect>
			<Routes>
				{routes.map(({ layout, component: Component, ...props }, _do_not_remove_) => {
					/* return <AppRoute key={0} {...props} layoutComponent={layoutRegistry(layout || defaultLayout)} />; */
					return <Route key={0} {...props} element={<Component />} />;
				})}
				{/* <AppRoute component={React.lazy(() => import("./page-error-404"))} layoutComponent={layoutRegistry(errorLayout)} /> */}
				<Route path="*" element={<React.Suspense fallback={<>...</>}>
					<PageError404 />
				</React.Suspense>} />
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
