import React from "react";
import { IDataField } from "@montr-core/models";
import { DataForm } from "@montr-core/components";
import { Spin } from "antd";
import { IDocument } from "@montr-docs/models";
import { DocumentMetadataService } from "@montr-docs/services";

interface IProps {
	data: IDocument;
}

interface IState {
	loading: boolean;
	fields?: IDataField[];
}

export class TabViewDocumentFields extends React.Component<IProps, IState> {

	private _documentMetadataService = new DocumentMetadataService();

	constructor(props: IProps) {
		super(props);

		this.state = {
			loading: true
		};
	}

	componentDidMount = async () => {
		await this.fetchData();
	};

	componentWillUnmount = async () => {
		await this._documentMetadataService.abort();
	};

	fetchData = async () => {
		const { data } = this.props;

		const dataView = await this._documentMetadataService.load(data.documentTypeUid);

		this.setState({ loading: false, fields: dataView.fields });
	};

	render = () => {
		const { data } = this.props,
			{ fields, loading } = this.state;

		return (
			<Spin spinning={loading}>
				<DataForm mode="View" fields={fields} data={data} />
			</Spin>
		);
	};
}
