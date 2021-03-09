import * as React from "react";
import { Page } from "@montr-core/components";
import { Spin, Button } from "antd";
import { RouteComponentProps } from "react-router-dom";
import { Translation } from "react-i18next";
import { AccountService } from "../services/account-service";
import { Locale } from "../module";

interface RouteProps {
	userId: string;
	code: string;
}

interface Props extends RouteComponentProps<RouteProps> {
}

interface State {
	loading: boolean;
}

export default class ConfirmEmail extends React.Component<Props, State> {

	private _accountService = new AccountService();

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
		await this._accountService.abort();
	};

	fetchData = async () => {
		const { userId, code } = this.props.match.params;

		const result = await this._accountService.confirmEmail({ userId, code });

		if (result.success) {
			this.setState({ loading: false });
		}
	};

	handleContinue = async () => {
		// todo: use route const, redirect to profile
		this.props.history.push("/dashboard");
	};

	render = () => {
		const { loading } = this.state;

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
