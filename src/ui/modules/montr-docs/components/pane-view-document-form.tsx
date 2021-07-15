import { DataForm } from "@montr-core/components";
import { IDataField } from "@montr-core/models";
import { IDocument } from "@montr-docs/models";
import { DocumentMetadataService } from "@montr-docs/services";
import { Spin } from "antd";
import React from "react";

interface Props {
	document: IDocument;
	mode?: "edit" | "view";
}

interface State {
	loading: boolean;
	fields?: IDataField[];
}

export default class PaneViewDocumentForm extends React.Component<Props, State> {

	private _documentMetadataService = new DocumentMetadataService();

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
		await this._documentMetadataService.abort();
	};

	fetchData = async (): Promise<void> => {
		const { document } = this.props;

		if (document.uid) {
			const dataView = await this._documentMetadataService.view(document.uid, null);

			this.setState({ loading: false, fields: dataView.fields });
		}
	};

	render = (): React.ReactNode => {
		const { document, mode } = this.props,
			{ fields, loading } = this.state;

		return (
			<Spin spinning={loading}>
				<DataForm mode={mode} fields={fields} data={document} />
			</Spin>
		);
	};
}
