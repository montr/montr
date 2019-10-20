import { SearchLocaleString, Settings, Dashboard } from "./pages";
import { IRoute } from "./models";
import { AppRouteRegistry } from "./services/app-routes-registry";

export const Routes: IRoute[] = [
	{ path: "/dashboard/", layout: "private", exact: true, component: Dashboard },
	{ path: "/locales/", layout: "private", exact: true, component: SearchLocaleString },
	{ path: "/settings/", layout: "private", exact: true, component: Settings },
];

AppRouteRegistry.add(Routes);
