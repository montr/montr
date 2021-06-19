import { Constants } from "@montr-core/constants";
import { AppRouteRegistry } from "@montr-core/services";

export const Api = {
	companyList: `${Constants.apiURL}/userCompany/list`,
	companyCreate: `${Constants.apiURL}/userCompany/create`,
};

export const Patterns = {
	// searchCompanies: "/companies",
};

export const Views = {
	gridSearchCompanies: "CompanySearch/Grid",
};

AppRouteRegistry.add([
	// { path: Patterns.searchCompanies, exact: true, component: React.lazy(() => import("./components/page-search-companies")) },
]);
