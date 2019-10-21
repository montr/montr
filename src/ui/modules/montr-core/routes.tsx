import { Home, SearchLocaleString, Settings, Dashboard } from "./pages";
import { IRoute } from "./models";
import { AppRouteRegistry } from "./services/";

export const Routes: IRoute[] = [
	{ path: "/", layout: "public", exact: true, component: Home },
	{ path: "/dashboard/", exact: true, component: Dashboard },
	{ path: "/locales/", exact: true, component: SearchLocaleString },
	{ path: "/settings/", exact: true, component: Settings },
];

AppRouteRegistry.add(Routes);
