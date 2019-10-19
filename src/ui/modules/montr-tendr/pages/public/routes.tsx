import * as React from "react";
import { Registration } from "../public";
import { Registration as CompanyRegistration } from "@montr-kompany/pages/public/";
import { AppRoute as Route } from "@montr-core/components";
import { Switch } from "react-router";

export const Routes = () => {
	return <Switch>
		<Route path="/register" exact component={Registration} />
		<Route path="/register/company" exact component={CompanyRegistration} />
	</Switch>
}
