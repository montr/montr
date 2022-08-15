import { Page } from "@montr-core/components";
import { Patterns } from "@montr-core/module";
import { Button, Spin } from "antd";
import * as React from "react";
import { Translation } from "react-i18next";
import { Navigate, useParams } from "react-router-dom";
import { Locale } from "../module";
import { AccountService } from "../services/account-service";

interface RouteProps {
	userId?: string;
	code?: string;
}

interface State {
	loading: boolean;
	navigateTo?: string;
}

export default class ConfirmEmail extends React.Component<unknown, State> {

	private _accountService = new AccountService();

	constructor(props: unknown) {
		super(props);

		this.state = {
			loading: true
		};
	}

	getRouteProps = (): RouteProps => {
		return useParams();
	};

	componentDidMount = async () => {
		await this.fetchData();
	};

	componentWillUnmount = async () => {
		await this._accountService.abort();
	};

	fetchData = async () => {
		const { userId, code } = this.getRouteProps();

		const result = await this._accountService.confirmEmail({ userId, code });

		if (result.success) {
			this.setState({ loading: false });
		}
	};

	handleContinue = async () => {
		this.setState({ navigateTo: Patterns.dashboard });
	};

	render = () => {
		const { loading, navigateTo } = this.state;

		if (navigateTo) {
			return <Navigate to={navigateTo} />;
		}

		return (
			<Translation ns={Locale.Namespace}>
				{(t) => <Page title={t("page.confirmEmail.title")}>
					<Spin spinning={loading}>

						{!loading && <p>Thank you for confirming your email.</p>}

						<Button disabled={loading} onClick={this.handleContinue}>{t("button.continue")}</Button>
					</Spin>
				</Page>}
			</Translation>
		);
	};
}
