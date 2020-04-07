import React from "react";
import { RouteComponentProps } from "react-router";
import { Spin, Tabs } from "antd";
import { PageHeader, Page } from "@montr-core/components";
import { INumerator } from "../models";
import { NumeratorService } from "../services";
import { RouteBuilder } from "../module";
import { TabEditNumerator } from "./";

interface IRouteProps {
	uid?: string;
	tabKey?: string;
}

interface IProps extends RouteComponentProps<IRouteProps> {
}

interface IState {
	loading: boolean;
	data?: INumerator;
}

export default class PageEditNumerator extends React.Component<IProps, IState> {

	private _numeratorService = new NumeratorService();

	constructor(props: IProps) {
		super(props);

		this.state = {
			loading: true
		};
	}

	componentDidMount = async () => {
		await this.fetchData();
	};

	componentDidUpdate = async (prevProps: IProps) => {
		if (this.props.match.params.uid !== prevProps.match.params.uid) {
			await this.fetchData();
		}
	};

	componentWillUnmount = async () => {
		await this._numeratorService.abort();
	};

	fetchData = async () => {
		const { uid } = this.props.match.params;

		const data = (uid)
			? await this._numeratorService.get(uid)
			: await this._numeratorService.create();

		this.setState({ loading: false, data });
	};

	handleDataChange = (data: INumerator) => {
		const { uid } = this.props.match.params;

		if (uid) {
			this.setState({ data });
		}
		else {
			const path = RouteBuilder.editNumerator(data.uid);

			this.props.history.push(path);
		}
	};

	// todo: use common component for all tabbed pages
	handleTabChange = (tabKey: string) => {
		const { uid } = this.props.match.params;

		const path = RouteBuilder.editNumerator(uid, tabKey);

		this.props.history.replace(path);
	};

	render = () => {
		const { tabKey } = this.props.match.params,
			{ loading, data } = this.state;

		const otherTabsDisabled = !data?.uid;

		return (
			<Page title={<>
				<PageHeader>{data?.name}</PageHeader>
			</>}>
				<Spin spinning={loading}>
					<Tabs size="small" defaultActiveKey={tabKey} onChange={this.handleTabChange}>
						<Tabs.TabPane key="info" tab="Информация">
							<TabEditNumerator data={data} onDataChange={this.handleDataChange} />
						</Tabs.TabPane>
						<Tabs.TabPane key="entities" tab="Использование" disabled={otherTabsDisabled}>
						</Tabs.TabPane>
						<Tabs.TabPane key="history" tab="История изменений" disabled={otherTabsDisabled}></Tabs.TabPane>
					</Tabs>
				</Spin>
			</Page>
		);
	};
}
