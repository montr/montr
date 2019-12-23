import React from "react";
import { DataForm } from "./";
import { MetadataService } from "../services";
import { IDataField, IApiResult } from "../models";
import { Spin } from "antd";

interface IProps {
	entityTypeCode: string;
}

interface IState {
	loading: boolean;
	fields?: IDataField[];
	data?: IDataField;
}

/* todo: rename to PaneEditMetadata */
export class PaneEditMetadataForm extends React.Component<IProps, IState> {

	private _metadataService = new MetadataService();

	constructor(props: IProps) {
		super(props);

		this.state = {
			loading: true
		};
	}

	componentDidMount = async () => {
		await this.fetchData();
	};

	fetchData = async () => {
		const dataView = await this._metadataService.load("Metadata/Edit");

		this.setState({ loading: false, fields: dataView.fields });
	};

	handleSubmit = async (values: IDataField): Promise<IApiResult> => {

		const { entityTypeCode } = this.props;

		if (false /* data.uid */) {
		}
		else {
			const result = await this._metadataService.insert({ entityTypeCode, item: values });

			return result;
		}

		return null;
	};

	render = () => {
		const // { data } = this.props,
			{ loading, fields, data } = this.state;

		return (
			<Spin spinning={loading}>
				<DataForm
					fields={fields}
					data={data}
					onSubmit={this.handleSubmit} />
			</Spin>
		);
	};
}
