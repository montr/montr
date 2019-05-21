import * as React from "react";
import { Route, Switch, generatePath } from "react-router";
import { SearchClassifier, EditClassifierType, EditClassifier, SearchClassifierType } from "./pages";
import { Guid } from "@montr-core/models";

export const Patterns = {
	editClassifierType: "/classifiers/edit/:uid/:tabKey?",

	editClassifier: "/classifiers/:typeCode/edit/:uid/:tabKey?",
};

export const RouteBuilder = {
	editClassifierType: (uid: Guid | string, tabKey?: string) => {
		return generatePath(Patterns.editClassifierType, { uid: uid.toString(), tabKey });
	},
	editClassifier: (typeCode: string, uid: Guid | string, tabKey?: string) => {
		return generatePath(Patterns.editClassifier, { typeCode, uid: uid.toString(), tabKey });
	},
};

export const Routes = () => {
	return (
		<Switch>
			<Route path="/classifiers/" exact component={SearchClassifierType} />
			<Route path="/classifiers/add" exact component={EditClassifierType} />
			<Route path={Patterns.editClassifierType} exact component={EditClassifierType} />

			<Route path="/classifiers/:typeCode/" exact component={SearchClassifier} />
			<Route path="/classifiers/:typeCode/add" exact component={EditClassifier} />
			<Route path={Patterns.editClassifier} exact component={EditClassifier} />
		</Switch>
	)
}
