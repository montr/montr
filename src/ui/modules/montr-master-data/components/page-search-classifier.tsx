import * as React from "react";
import { useParams } from "react-router-dom";
import { PaneSearchClassifier } from ".";

interface RouteProps {
	typeCode?: string;
}

export default function SearchClassifier() {

	const { typeCode } = useParams();

	return (
		<PaneSearchClassifier mode="page" typeCode={typeCode} />
	);
}
