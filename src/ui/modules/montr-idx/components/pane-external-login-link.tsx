import React from "react";
import { RouteComponentProps } from "react-router";
import { Spin } from "antd";
import { Translation } from "react-i18next";
import { Page } from "@montr-core/components";
import { OperationService } from "@montr-core/services";
import { ProfileService } from "../services";
import { Locale, Patterns } from "../module";

interface Props extends RouteComponentProps {
}

interface State {
	loading: boolean;
}

export default class PaneExternalLoginLink extends React.Component<Props, State> {

	private _operation = new OperationService();
	private _profileService = new ProfileService();

	constructor(props: Props) {
		super(props);

		this.state = {
			loading: true
		};
	}

	componentDidMount = async () => {
		await this.fetchData();
	};

	componentWillUnmount = async () => {
		await this._profileService.abort();
	};

	fetchData = async () => {

		await this._operation.execute(() => {
			return this._profileService.linkLoginCallback();
		});

		this.setState({ loading: false });

		this.props.history.push(Patterns.profileExternalLogin);
	};

	render = () => {
		const { loading } = this.state;

		return (
			<Translation ns={Locale.Namespace}>
				{(t) => <Page title={t("page.linkLogin.title")}>
					<Spin spinning={loading}>

					</Spin>
				</Page>}
			</Translation>
		);
	};
}
