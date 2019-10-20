import { Registration } from "../public";
import { Registration as CompanyRegistration } from "@montr-kompany/pages/public/";
import { IRoute } from "@montr-core/models";
import { AppRouteRegistry } from "@montr-core/services/app-routes-registry";

export const Routes: IRoute[] = [
	{ path: "/register/", layout: "public", exact: true, component: Registration },
	{ path: "/register/company/", layout: "public", exact: true, component: CompanyRegistration },
];

AppRouteRegistry.add(Routes);
