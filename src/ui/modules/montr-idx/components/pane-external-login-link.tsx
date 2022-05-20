import { Page } from "@montr-core/components";
import { OperationService } from "@montr-core/services";
import { Spin } from "antd";
import React from "react";
import { Translation } from "react-i18next";
import { useNavigate } from "react-router-dom";
import { Locale, Patterns } from "../module";
import { ProfileService } from "../services";

interface State {
	loading: boolean;
}

export default class PaneExternalLoginLink extends React.Component<unknown, State> {

	private _operation = new OperationService();
	private _profileService = new ProfileService();

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
		await this._profileService.abort();
	};

	fetchData = async () => {

		await this._operation.execute(() => {
			return this._profileService.linkLoginCallback();
		});

		this.setState({ loading: false });

		const navigate = useNavigate();

		navigate(Patterns.profileExternalLogin);
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
