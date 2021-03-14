import React from "react";
import { IDataField } from "@montr-core/models";
import { DataForm } from "@montr-core/components";
import { Spin } from "antd";
import { IDocument } from "@montr-docs/models";
import { DocumentMetadataService } from "@montr-docs/services";

interface Props {
	data: IDocument;
}

interface State {
	loading: boolean;
	fields?: IDataField[];
}

export class TabViewDocumentFields extends React.Component<Props, State> {

	private _documentMetadataService = new DocumentMetadataService();

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
		if (this.props.data !== prevProps.data) {
			await this.fetchData();
		}
	};

	componentWillUnmount = async () => {
		await this._documentMetadataService.abort();
	};

	fetchData = async () => {
		const { data } = this.props;

		if (data.documentTypeUid) {
			const dataView = await this._documentMetadataService.load(data.documentTypeUid);

			this.setState({ loading: false, fields: dataView.fields });
		}
	};

	render = () => {
		const { data } = this.props,
			{ fields, loading } = this.state;

		return (
			<Spin spinning={loading}>
				<DataForm mode="view" fields={fields} data={data} />
			</Spin>
		);
	};
}
