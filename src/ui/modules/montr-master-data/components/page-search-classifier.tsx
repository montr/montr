import * as React from "react";
import { RouteComponentProps } from "react-router";
import { PaneSearchClassifier } from ".";

interface RouteProps {
	typeCode: string;
}

interface Props extends RouteComponentProps<RouteProps> {
}

interface State {
}

export default class SearchClassifier extends React.Component<Props, State> {
	render() {
		return (
			<PaneSearchClassifier mode="Page" typeCode={this.props.match.params.typeCode} />
		);
	}
}
