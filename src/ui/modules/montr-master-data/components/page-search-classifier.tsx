import * as React from "react";
import { useParams } from "react-router-dom";
import { PaneSearchClassifier } from ".";

interface RouteProps {
	typeCode?: string;
}

export default function SearchClassifier() {

	function getRouteProps(): RouteProps {
		return useParams();
	}

	const { typeCode } = getRouteProps();

	return (
		<PaneSearchClassifier mode="page" typeCode={typeCode} />
	);
}
