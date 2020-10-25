import React from "react";
import { RouteComponentProps } from "react-router";
import { Spin } from "antd";
import { DataView } from "@montr-core/models";
import { DataTabs, Page } from "@montr-core/components";
import { RouteBuilder, Views } from "../module";
import { Guid } from "@montr-core/models";
import { MetadataService } from "@montr-core/services";
import { IDocument } from "../models";

interface IRouteProps {
	uid?: string;
	tabKey?: string;
}

interface IProps extends RouteComponentProps<IRouteProps> {
}

interface IState {
	loading: boolean;
	dataView?: DataView<IDocument>;
}

export default class PageEditDocumentType extends React.Component<IProps, IState> {

	private _metadataService = new MetadataService();

	constructor(props: IProps) {
		super(props);

		this.state = {
			loading: false
		};
	}

	componentDidMount = async () => {
		await this.fetchData();
	};

	componentWillUnmount = async () => {
		await this._metadataService.abort();
	};

	fetchData = async () => {
		const dataView = await this._metadataService.load(Views.documentTypeTabs);

		this.setState({ loading: false, dataView });
	};

	handleTabChange = (tabKey: string) => {
		const { uid } = this.props.match.params;

		const path = RouteBuilder.editDocumentType(uid, tabKey);

		this.props.history.replace(path);
	};

	render = () => {
		const { uid, tabKey } = this.props.match.params,
			{ loading, dataView } = this.state;

		if (Guid.isValid(uid) == false) {
			return <span>Not a valid identifier</span>;
		}

		const entityUid = new Guid(uid);

		return (
			<Page title={`${uid}`}>
				<Spin spinning={loading}>

					<DataTabs
						tabKey={tabKey}
						panes={dataView?.panes}
						onTabChange={this.handleTabChange}
						// disabled={(_, index) => index > 0 && !data?.uid}
						tabProps={{
							// data,
							// onDataChange: this.handleDataChange,
							entityTypeCode: `DocumentType`,
							entityUid: entityUid, // data?.uid
							entityTypeUid: entityUid // todo: rename to entityUid?
						}}
					/>

				</Spin>
			</Page>
		);
	};
}
