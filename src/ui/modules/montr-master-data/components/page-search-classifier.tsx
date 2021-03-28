import * as React from "react";
import { RouteComponentProps } from "react-router";
import { PaneSearchClassifier } from ".";

interface RouteProps {
	typeCode: string;
}

interface Props extends RouteComponentProps<RouteProps> {
}

export default class SearchClassifier extends React.Component<Props> {
	render(): React.ReactNode {
		return (
			<PaneSearchClassifier mode="page" typeCode={this.props.match.params.typeCode} />
		);
	}
}
