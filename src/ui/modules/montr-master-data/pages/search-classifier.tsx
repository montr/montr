import * as React from "react";
import { RouteComponentProps } from "react-router";
import { PaneSearchClassifier, ClassifierBreadcrumb } from "../components";
import { IClassifierType } from "@montr-master-data/models";
import { CompanyContextProps } from "@kompany/components";
import { ClassifierTypeService } from "@montr-master-data/services";

interface IRouteProps {
	typeCode: string;
}

interface IProps extends CompanyContextProps, RouteComponentProps<IRouteProps> {
}

interface IState {
	types: IClassifierType[];
	type?: IClassifierType;
}

export class SearchClassifier extends React.Component<IProps, IState> {

	_classifierTypeService = new ClassifierTypeService();

	constructor(props: IProps) {
		super(props);

		this.state = {
			types: [],
		};
	}

	componentDidMount = async () => {
		await this.loadClassifierTypes();
	}

	componentDidUpdate = async (prevProps: IProps) => {
		if (this.props.currentCompany !== prevProps.currentCompany) {
			// todo: check if selected type belongs to company (show 404)
			await this.loadClassifierTypes();
		}
		else if (this.props.match.params.typeCode !== prevProps.match.params.typeCode) {
		}
	}

	componentWillUnmount = async () => {
		await this._classifierTypeService.abort();
	}

	loadClassifierTypes = async () => {
		const { currentCompany } = this.props;

		if (currentCompany) {
			const types = (await this._classifierTypeService.list(currentCompany.uid)).rows;

			this.setState({ types });

			// await this.loadClassifierType();
		}
	}

	render() {
		const { types, type } = this.state;

		// if (!type) return null;

		return (<>
			<ClassifierBreadcrumb type={type} types={types} />
			<PaneSearchClassifier typeCode={this.props.match.params.typeCode} />
		</>);
	}
}
