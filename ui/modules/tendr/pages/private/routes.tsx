import * as React from "react";
import { Route } from "react-router";
import { Dashboard, SearchEvents, SelectEventTemplate, EditEvent } from ".";
import { Contractors } from "@kompany/pages/private";

export const Routes = () => {
	return <>
		<Route path="/" exact component={() => <Dashboard />} />
		<Route path="/events" exact component={() => <SearchEvents />} />
		<Route path="/events/new" component={() => <SelectEventTemplate />} />
		<Route path="/events/edit/:id" component={({ match }: any) => <EditEvent {...match} />} />
		<Route path="/contractors" exact component={Contractors} />
	</>
}
