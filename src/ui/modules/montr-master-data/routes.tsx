import * as React from "react";
import { Switch, generatePath } from "react-router";
import { SearchClassifier, EditClassifierType, EditClassifier, SearchClassifierType } from "./pages";
import { AppRoute as Route } from "@montr-core/components";
import { Guid } from "@montr-core/models";

export const Patterns = {
	editClassifierType: "/classifiers/edit/:uid/:tabKey?",

	addClassifier: "/classifiers/:typeCode/add/:parentUid?",
	editClassifier: "/classifiers/:typeCode/edit/:uid/:tabKey?",
};

export const RouteBuilder = {
	editClassifierType: (uid: Guid | string, tabKey?: string) => {
		return generatePath(Patterns.editClassifierType, { uid: uid.toString(), tabKey });
	},
	addClassifier: (typeCode: string, parentUid: Guid | string) => {
		return generatePath(Patterns.addClassifier, { typeCode, parentUid: parentUid ? parentUid.toString() : null });
	},
	editClassifier: (typeCode: string, uid: Guid | string, tabKey?: string) => {
		return generatePath(Patterns.editClassifier, { typeCode, uid: uid.toString(), tabKey });
	},
};

export const Routes = () => {
	return <Switch>
		<Route path="/classifiers/" layout="private" exact component={SearchClassifierType} />
		<Route path="/classifiers/add" layout="private" exact component={EditClassifierType} />
		<Route path={Patterns.editClassifierType} layout="private" exact component={EditClassifierType} />

		<Route path="/classifiers/:typeCode/" layout="private" exact component={SearchClassifier} />
		<Route path={Patterns.addClassifier} layout="private" exact component={EditClassifier} />
		<Route path={Patterns.editClassifier} layout="private" exact component={EditClassifier} />
	</Switch>
}
