import { EmptyFieldView } from "@montr-core/components";
import { IIndexer } from "@montr-core/models";
import { DataHelper } from "@montr-core/services/data-helper";
import { ClassifierService } from "@montr-master-data/services";
import * as React from "react";
import { Classifier, IClassifierField } from "../models";

interface Props {
	value?: string;
	field: IClassifierField;
	data: IIndexer;
}

interface State {
	loading: boolean;
	item?: Classifier;
}

export class ClassifierView extends React.Component<Props, State> {

	static getDerivedStateFromProps(nextProps: any): void {
		// Should be a controlled component.
		if ("value" in nextProps) {
			return nextProps.value ?? null;
		}
		return null;
	}

	private _classifierService = new ClassifierService();

	constructor(props: Props) {
		super(props);

		this.state = {
			loading: true,
		};
	}

	componentDidMount = async (): Promise<void> => {
		await this.fetchData();
	};

	fetchData = async (): Promise<void> => {
		const { field, data } = this.props;

		const value = DataHelper.indexer(data, field.key, undefined);

		const item = value ? await this._classifierService.get(field.props.typeCode, value) : null;

		this.setState({ loading: false, item });
	};

	render = (): React.ReactNode => {
		const { item } = this.state;

		return (item) ? item.name : <EmptyFieldView />;
	};
}
