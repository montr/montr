import React from "react";
import { DataForm } from "./";
import { MetadataService } from "../services";
import { IDataField, IApiResult, Guid } from "../models";
import { Spin } from "antd";

interface IProps {
	entityTypeCode: string;
	uid?: Guid;
	onSuccess?: () => void;
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
		const { entityTypeCode, uid } = this.props;

		const data = (uid) ? await this._metadataService.get(entityTypeCode, uid) : null;

		const dataView = await this._metadataService.load("Metadata/Edit");

		this.setState({ loading: false, data, fields: dataView.fields });
	};

	handleSubmit = async (values: IDataField): Promise<IApiResult> => {
		const { entityTypeCode, uid, onSuccess } = this.props;

		let result;

		if (uid) {
			result = await this._metadataService.update(entityTypeCode, { uid, ...values });
		}
		else {
			result = await this._metadataService.insert({ entityTypeCode, item: values });
		}

		if (result.success && onSuccess) {
			onSuccess();
		}

		return result;
	};

	render = () => {
		const { loading, fields, data } = this.state;

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
