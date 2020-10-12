import React from "react";
import { RouteComponentProps } from "react-router";
import { Spin, Tabs } from "antd";
import { Page, PaneSearchMetadata, PaneSearchEntityStatuses } from "@montr-core/components";
import { RouteBuilder } from "../module";
import { PaneSearchAutomation } from "@montr-automate/components/pane-search-automation";
import { Guid } from "@montr-core/models";

interface IRouteProps {
	uid?: string;
	tabKey?: string;
}

interface IProps extends RouteComponentProps<IRouteProps> {
}

interface IState {
	loading: boolean;
}

export default class PageEditDocumentType extends React.Component<IProps, IState> {

	constructor(props: IProps) {
		super(props);

		this.state = {
			loading: false
		};
	}

	handleTabChange = (tabKey: string) => {
		const { uid } = this.props.match.params;

		const path = RouteBuilder.editDocumentType(uid, tabKey);

		this.props.history.replace(path);
	};

	render = () => {
		const { uid, tabKey } = this.props.match.params,
			{ loading } = this.state;

		if (Guid.isValid(uid) == false) {
			return <span>Not a valid identifier</span>
		}

		const entityUid = new Guid(uid);

		const otherTabsDisabled = !uid;

		return (
			<Page title={`${uid}`}>
				<Spin spinning={loading}>
					<Tabs size="small" defaultActiveKey={tabKey} onChange={this.handleTabChange}>
						<Tabs.TabPane key="common" tab="Информация">
						</Tabs.TabPane>
						<Tabs.TabPane key="statuses" tab="Statuses">
							<PaneSearchEntityStatuses entityTypeCode={`DocumentType`} entityUid={entityUid} />
						</Tabs.TabPane>
						<Tabs.TabPane key="fields" tab="Поля">
							<PaneSearchMetadata entityTypeCode={`DocumentType`} entityUid={entityUid} />
						</Tabs.TabPane>
						<Tabs.TabPane key="automation" tab="Automations">
							<PaneSearchAutomation entityTypeCode={`DocumentType`} entityTypeUid={entityUid} />
						</Tabs.TabPane>
					</Tabs>
				</Spin>
			</Page>
		);
	};
}
