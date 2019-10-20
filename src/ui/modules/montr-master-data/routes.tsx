import { generatePath } from "react-router";
import { SearchClassifier, EditClassifierType, EditClassifier, SearchClassifierType } from "./pages";
import { Guid, IRoute } from "@montr-core/models";
import { AppRouteRegistry } from "@montr-core/services/app-routes-registry";

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

export const Routes: IRoute[] = [
	{ path: "/classifiers/", layout: "private", exact: true, component: SearchClassifierType },
	{ path: "/classifiers/add/", layout: "private", exact: true, component: EditClassifierType },
	{ path: Patterns.editClassifierType, layout: "private", exact: true, component: EditClassifierType },

	{ path: "/classifiers/:typeCode/", layout: "private", exact: true, component: SearchClassifier },
	{ path: Patterns.addClassifier, layout: "private", exact: true, component: EditClassifier },
	{ path: Patterns.editClassifier, layout: "private", exact: true, component: EditClassifier }
];

AppRouteRegistry.add(Routes);
