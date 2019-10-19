import * as React from "react";
import { Switch, generatePath } from "react-router";
import { SearchEvents, SelectEventTemplate, EditEvent } from "../private";
import { AppRoute as Route } from "@montr-core/components";

export const Patterns = {
	editEvent: "/events/edit/:uid/:tabKey?",
};

export const RouteBuilder = {
	editEvent: (uid: string, tabKey?: string) => {
		return generatePath(Patterns.editEvent, { uid, tabKey });
	},
};

export const Routes = () => {
	return <Switch>
		<Route path="/events" layout="private" exact component={SearchEvents} />
		<Route path="/events/new" layout="private" component={SelectEventTemplate} />
		<Route path={Patterns.editEvent} layout="private" exact component={EditEvent} />
	</Switch>
}
