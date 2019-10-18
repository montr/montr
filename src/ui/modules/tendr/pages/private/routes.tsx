import * as React from "react";
import { Route, Switch, generatePath } from "react-router";
import { Dashboard, SearchEvents, SelectEventTemplate, EditEvent } from ".";

export const Patterns = {
	editEvent: "/events/edit/:uid/:tabKey?",
};

export const RouteBuilder = {
	editEvent: (uid: string, tabKey?: string) => {
		return generatePath(Patterns.editEvent, { uid, tabKey });
	},
};

export const Routes = () => {
	return <>
		<Route path="/" exact component={Dashboard} />

		<Switch>
			<Route path="/events" exact component={SearchEvents} />
			<Route path="/events/new" component={SelectEventTemplate} />
			<Route path={Patterns.editEvent} exact component={EditEvent} />
		</Switch>
	</>
}
