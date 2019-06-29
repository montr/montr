import * as React from "react";
import { Route, Switch } from "react-router";
import { Dashboard, SearchEvents, SelectEventTemplate, EditEvent } from ".";

export const Routes = () => {
	return <>
		<Route path="/" exact component={() => <Dashboard />} />

		<Switch>
			<Route path="/events" exact component={() => <SearchEvents />} />
			<Route path="/events/new" component={() => <SelectEventTemplate />} />
			<Route path="/events/edit/:id" component={({ match }: any) => <EditEvent {...match} />} />
		</Switch>
	</>
}
