import * as React from "react";
import { useParams } from "react-router-dom";
import { PaneSearchClassifier } from ".";

interface RouteProps {
	typeCode: string;
}

export default class SearchClassifier extends React.Component {

	getRouteProps = (): RouteProps => {
		return useParams();
	};

	render(): React.ReactNode {

		const { typeCode } = this.getRouteProps();

		return (
			<PaneSearchClassifier mode="page" typeCode={typeCode} />
		);
	}
}
