import React from "react";
import { RouteComponentProps } from "react-router";
import { Spin } from "antd";
import { DataTabs, Page } from "@montr-core/components";
import { MetadataService } from "@montr-core/services";
import { Guid, DataView } from "@montr-core/models";
import { RouteBuilder, Views } from "../module";
import { Process } from "../models/process";

interface RouteProps {
	uid?: string;
	tabKey?: string;
}

interface Props extends RouteComponentProps<RouteProps> {
}

interface State {
	loading: boolean;
	dataView?: DataView<Process>;
}

export default class PageEditProcess extends React.Component<Props, State> {

	private _metadataService = new MetadataService();

	constructor(props: Props) {
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
		const dataView = await this._metadataService.load(Views.processTabs);

		this.setState({ loading: false, dataView });
	};

	handleTabChange = (tabKey: string) => {
		const { uid } = this.props.match.params;

		const path = RouteBuilder.editProcess(uid, tabKey);

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
							entityTypeCode: `Process`,
							entityUid: entityUid // data?.uid
						}}
					/>

				</Spin>
			</Page>
		);
	};
}
