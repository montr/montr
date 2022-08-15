import { Layout } from "@montr-core/constants";
import { AppRouteRegistry } from "@montr-core/services/app-route-registry";
import React from "react";
import { useRoutes } from "react-router-dom";

const PublicLayout = React.lazy(() => import("./components/public-layout"));
const PrivateLayout = React.lazy(() => import("./components/private-layout"));
const AuthLayout = React.lazy(() => import("./components/auth-layout"));

const PageProfile = React.lazy(() => import("@montr-idx/components/page-profile"));
const PageError404 = React.lazy(() => import("@montr-core/components/page-error-404"));

export function AppRoutes() {

	const routes = [
		{
			element: <PublicLayout />,
			children: AppRouteRegistry.get(Layout.public)
		},
		{
			element: <PrivateLayout />,
			children: [
				{
					element: < PageProfile />,
					children: AppRouteRegistry.get(Layout.profile)
				},
				...AppRouteRegistry.get(Layout.private)
			]
		},
		{
			element: <AuthLayout />,
			children: AppRouteRegistry.get(Layout.auth)
		},
		{
			path: "*",
			element: <PageError404 />
		},
	];

	return useRoutes(routes);
}
