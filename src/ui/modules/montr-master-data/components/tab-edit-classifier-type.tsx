import { DataForm } from "@montr-core/components";
import { ApiResult, IDataField } from "@montr-core/models";
import { MetadataService } from "@montr-core/services";
import { Spin } from "antd";
import * as React from "react";
import { Navigate } from "react-router";
import { ClassifierType } from "../models";
import { RouteBuilder, Views } from "../module";
import { ClassifierTypeService } from "../services";

interface Props {
	data: ClassifierType;
	onDataChange?: (values: ClassifierType) => void;
}

interface State {
	loading: boolean;
	fields?: IDataField[];
	redirect?: string;
}

export default class TabEditClassifierType extends React.Component<Props, State> {
	private _metadataService = new MetadataService();
	private _classifierTypeService = new ClassifierTypeService();

	constructor(props: Props) {
		super(props);

		this.state = {
			loading: true
		};
	}

	componentDidMount = async (): Promise<void> => {
		await this.fetchData();
	};

	componentWillUnmount = async (): Promise<void> => {
		await this._metadataService.abort();
		await this._classifierTypeService.abort();
	};

	fetchData = async (): Promise<void> => {
		const dataView = await this._metadataService.load(Views.classifierTypeForm);

		this.setState({ loading: false, fields: dataView.fields });
	};

	save = async (values: ClassifierType): Promise<ApiResult> => {

		const { data, onDataChange } = this.props;

		if (data.uid) {
			const updated = { uid: data.uid, ...values };

			const result = await this._classifierTypeService.update(updated);

			if (result.success) {
				if (onDataChange) await onDataChange(updated);
			}

			return result;
		}
		else {
			const result = await this._classifierTypeService.insert(values);

			if (result.success) {
				this.setState({ redirect: RouteBuilder.editClassifierType(result.uid) });
			}

			return result;
		}
	};

	render = (): React.ReactNode => {
		const { data } = this.props,
			{ loading, redirect, fields } = this.state;

		if (redirect) {
			// this.setState({ redirect: null });
			return <Navigate to={redirect} />;
		}

		return (
			<Spin spinning={loading}>
				<DataForm fields={fields} data={data} onSubmit={this.save} />
			</Spin>
		);
	};
}
