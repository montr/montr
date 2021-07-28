import { DataForm } from "@montr-core/components";
import { ApiResult, IDataField } from "@montr-core/models";
import { Spin } from "antd";
import React from "react";
import { IDocument } from "../models";
import { Api, Views } from "../module";
import { DocumentService } from "../services";

interface Props {
	document: IDocument;
	mode?: "edit" | "view";
}

interface State {
	loading: boolean;
	fields?: IDataField[];
}

export default class PaneViewDocumentForm extends React.Component<Props, State> {

	private readonly documentService = new DocumentService();

	constructor(props: Props) {
		super(props);

		this.state = {
			loading: true
		};
	}

	componentDidMount = async (): Promise<void> => {
		await this.fetchData();
	};

	componentDidUpdate = async (prevProps: Props): Promise<void> => {
		if (this.props.document !== prevProps.document) {
			await this.fetchData();
		}
	};

	componentWillUnmount = async (): Promise<void> => {
		await this.documentService.abort();
	};

	fetchData = async (): Promise<void> => {
		const { document } = this.props;

		if (document.uid) {
			const dataView = await this.documentService.metadata(Views.documentForm, document.uid);

			this.setState({ loading: false, fields: dataView.fields });
		}
	};

	handleSubmit = async (values: IDocument): Promise<ApiResult> => {
		const { document } = this.props;

		return await this.documentService.post(Api.documentUpdateForm, {
			documentUid: document.uid, fields: { ...values }
		});
	};

	render = (): React.ReactNode => {
		const { document, mode = "view" } = this.props,
			{ fields, loading } = this.state;

		return (
			<Spin spinning={loading}>
				<DataForm
					mode={mode}
					fields={fields}
					data={document.fields}
					onSubmit={this.handleSubmit} />
			</Spin>
		);
	};
}
