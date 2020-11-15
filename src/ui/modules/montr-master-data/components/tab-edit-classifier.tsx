import * as React from "react";
import { Spin } from "antd";
import { IDataField, ApiResult } from "@montr-core/models";
import { DataForm } from "@montr-core/components";
import { ClassifierService, ClassifierMetadataService } from "../services";
import { IClassifier, IClassifierType } from "../models";
import { Views } from "@montr-master-data/module";

interface Props {
	type: IClassifierType;
	data: IClassifier;
	onDataChange?: (values: IClassifier) => void;
}

interface State {
	loading: boolean;
	fields?: IDataField[];
}

export default class TabEditClassifier extends React.Component<Props, State> {

	private _classifierMetadataService = new ClassifierMetadataService();
	private _classifierService = new ClassifierService();

	constructor(props: Props) {
		super(props);

		this.state = {
			loading: true
		};
	}

	componentDidMount = async () => {
		await this.fetchData();
	};

	componentDidUpdate = async (prevProps: Props) => {
		if (this.props.type !== prevProps.type) {
			await this.fetchData();
		}
	};

	componentWillUnmount = async () => {
		await this._classifierMetadataService.abort();
		await this._classifierService.abort();
	};

	fetchData = async () => {
		const { type } = this.props;

		if (type) {

			const dataView = await this._classifierMetadataService.load(type.code, Views.classifierForm);

			/* const fields = dataView.fields;

			const parentUidField = fields.find(x => x.key == "parentUid") as IClassifierField;

			if (parentUidField) {
				parentUidField.typeCode = type.code;
				// parentUidField.treeUid = treeUid;
			} */

			this.setState({ loading: false, fields: dataView.fields });
		}
	};

	save = async (values: IClassifier): Promise<ApiResult> => {
		const { type, data, onDataChange } = this.props;

		if (data.uid) {
			const updated = { uid: data.uid, ...values };

			const result = await this._classifierService.update(type.code, updated);

			if (result.success) {
				if (onDataChange) await onDataChange(updated);
			}

			return result;
		}
		else {
			const result = await this._classifierService.insert(type.code, values);

			if (result.success) {
				if (onDataChange) await onDataChange(result);
			}

			return result;
		}
	};

	render = () => {
		const { data } = this.props,
			{ fields, loading } = this.state;

		return (
			<Spin spinning={loading}>
				<DataForm fields={fields} data={data} onSubmit={this.save} />
			</Spin>
		);
	};
}
