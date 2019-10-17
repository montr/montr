import React from "react";
import { Route, Switch, generatePath } from "react-router";
import { SearchLocaleString, Settings } from "./pages";

export const Routes = () => {
	return (
		<Switch>
			<Route path="/locales/" exact component={SearchLocaleString} />
			<Route path="/settings/" exact component={Settings} />
		</Switch>
	)
}
