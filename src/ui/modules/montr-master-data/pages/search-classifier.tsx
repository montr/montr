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

export class SearchClassifier extends React.Component<IProps, IState> {
	componentDidUpdate = async (prevProps: IProps) => {
		if (this.props.match.params.typeCode !== prevProps.match.params.typeCode) {
		}
	}

	render() {
		return (
			<PaneSearchClassifier typeCode={this.props.match.params.typeCode} />
		);
	}
}
