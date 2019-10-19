import React from "react";
import { Switch, generatePath } from "react-router";
import { SearchLocaleString, Settings, Dashboard } from "./pages";
import { AppRoute as Route } from "./components";

export const Routes = () => {
	return <Switch>
		<Route path="/dashboard" layout="private" exact component={Dashboard} />
		<Route path="/locales/" layout="private" exact component={SearchLocaleString} />
		<Route path="/settings/" layout="private" exact component={Settings} />
	</Switch>
}
