import * as React from "react";
import { RouteComponentProps } from "react-router";
import { PaneSearchClassifier } from "../components";

interface IRouteProps {
	typeCode: string;
}

interface IProps extends RouteComponentProps<IRouteProps> {
}

interface IState {
}

export default class SearchClassifier extends React.Component<IProps, IState> {
	render() {
		return (
			<PaneSearchClassifier mode="Page" typeCode={this.props.match.params.typeCode} />
		);
	}
}
