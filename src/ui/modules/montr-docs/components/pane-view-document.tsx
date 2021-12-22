import { DataForm } from "@montr-core/components";
import { Guid, IDataField } from "@montr-core/models";
import { IDocument } from "@montr-docs/models";
import { DocumentService } from "@montr-docs/services";
import { Spin, Tag } from "antd";
import React from "react";
import { Link } from "react-router-dom";
import { Views } from "../module";

interface Props {
	entityTypeCode: string;
	entityUid: Guid;
}

interface State {
	loading: boolean;
	document?: IDocument;
	fields?: IDataField[];
}

export default class PaneViewDocument extends React.Component<Props, State> {

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

	componentWillUnmount = async (): Promise<void> => {
		await this.documentService.abort();
	};

	fetchData = async (): Promise<void> => {
		const { entityUid } = this.props;

		const document = await this.documentService.get(entityUid);

		const dataView = await this.documentService.metadata(Views.documentInfo, document.uid);

		this.setState({ loading: false, document, fields: dataView.fields });
	};

	render = (): React.ReactNode => {
		const { loading, document, fields } = this.state;

		return (
			<Spin spinning={loading}>

				<DataForm
					mode="view"
					layout="vertical"
					fields={fields}
					data={document} />

				<Tag>Document</Tag>
				<Link to={document?.url} >{document?.documentNumber}</Link>
			</Spin>
		);
	};
}
