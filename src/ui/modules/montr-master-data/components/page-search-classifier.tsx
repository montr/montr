import * as React from "react";
import { RouteComponentProps, useParams } from "react-router";
import { PaneSearchClassifier } from ".";

interface RouteProps {
	typeCode: string;
}

interface Props extends RouteComponentProps<RouteProps> {
}

export default class SearchClassifier extends React.Component<Props> {
	render(): React.ReactNode {

		const { typeCode } = useParams();

		return (
			<PaneSearchClassifier mode="page" typeCode={typeCode} />
		);
	}
}
