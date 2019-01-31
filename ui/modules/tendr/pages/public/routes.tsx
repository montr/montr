import * as React from "react";
import { Route } from "react-router";
import { Registration } from ".";
import { Registration as CompanyRegistration } from "@kompany/pages/public/";

export const Routes = () => {
	return <>
		<Route path="/register" exact component={Registration} />
		<Route path="/register/company" exact component={CompanyRegistration} />
	</>
}
