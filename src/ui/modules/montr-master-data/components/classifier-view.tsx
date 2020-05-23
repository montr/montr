import * as React from "react";
import { IClassifierField, IClassifier } from "../models";
import { IIndexer } from "@montr-core/models";
import { DataHelper } from "@montr-core/services";
import { ClassifierService } from "@montr-master-data/services";
import { EmptyFieldView } from "@montr-core/components";

interface IProps {
	value?: string;
	field: IClassifierField;
	data: IIndexer;
}

interface IState {
	loading: boolean;
	item?: IClassifier;
}

export class ClassifierView extends React.Component<IProps, IState> {

	static getDerivedStateFromProps(nextProps: any) {
		// Should be a controlled component.
		console.log(nextProps);
		if ("value" in nextProps) {
			return nextProps.value ?? null;
		}
		return null;
	}

	private _classifierService = new ClassifierService();

	constructor(props: IProps) {
		super(props);

		this.state = {
			loading: true,
		};
	}

	componentDidMount = async () => {
		await this.fetchData();
	};

	fetchData = async () => {
		const { field, data } = this.props;

		const value = DataHelper.indexer(data, field.key, undefined);

		const item = await this._classifierService.get(field.props.typeCode, value);

		this.setState({ loading: false, item });
	};

	render = () => {
		const { item } = this.state;

		return (item) ? item.name : <EmptyFieldView />;
	};
}
