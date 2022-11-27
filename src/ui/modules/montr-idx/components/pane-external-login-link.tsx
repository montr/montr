import { Page } from "@montr-core/components";
import { OperationService } from "@montr-core/services";
import { Spin } from "antd";
import React from "react";
import { Translation } from "react-i18next";
import { Navigate } from "react-router-dom";
import { Locale, Patterns } from "../module";
import { ProfileService } from "../services/profile-service";

interface State {
	loading: boolean;
	navigateTo?: string;
}

export default class PaneExternalLoginLink extends React.Component<unknown, State> {

	private readonly operation = new OperationService();
	private readonly profileService = new ProfileService();

	constructor(props: unknown) {
		super(props);

		this.state = {
			loading: true
		};
	}

	componentDidMount = async () => {
		await this.fetchData();
	};

	componentWillUnmount = async () => {
		await this.profileService.abort();
	};

	fetchData = async () => {

		await this.operation.execute(() => {
			return this.profileService.linkLoginCallback();
		});

		this.setState({ loading: false, navigateTo: Patterns.profileExternalLogin });
	};

	render = () => {
		const { loading, navigateTo } = this.state;

		if (navigateTo) {
			return <Navigate to={navigateTo} />;
		}

		return (
			<Translation ns={Locale.Namespace}>
				{(t) => <Page title={t("page.linkLogin.title") as string}>
					<Spin spinning={loading}>

					</Spin>
				</Page>}
			</Translation>
		);
	};
}
