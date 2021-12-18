import { Guid } from "@montr-core/models";
import { IDocument } from "@montr-docs/models";
import { DocumentService } from "@montr-docs/services";
import { Spin } from "antd";
import React from "react";
import { Link } from "react-router-dom";

interface Props {
	entityTypeCode: string;
	entityUid: Guid;
}

interface State {
	loading: boolean;
	document?: IDocument;
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

		this.setState({ loading: false, document });
	};

	render = (): React.ReactNode => {
		const { loading, document } = this.state;

		return <Spin spinning={loading}>
			<code>{JSON.stringify(document)}</code>

			<br />
			<br />

			<Link to={document?.url} >{document?.documentNumber}</Link>
		</Spin>;
	};
}
