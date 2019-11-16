import React from "react";
import { AppRouteRegistry } from "./services";
import { Layout } from "./constants";

import "./i18n";
import "./index.less"

AppRouteRegistry.add([
	{ path: "/", layout: Layout.public, exact: true, component: React.lazy(() => import("./pages/home")) },

	{ path: "/dashboard/", exact: true, component: React.lazy(() => import("./pages/dashboard")) },
	{ path: "/locales/", exact: true, component: React.lazy(() => import("./pages/search-locale-string")) },
	{ path: "/settings/", exact: true, component: React.lazy(() => import("./pages/settings")) },
]);
