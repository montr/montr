import * as React from "react";
import { Spin, Button } from "antd";
import { Patterns } from "@montr-core/module";
import { Page } from "@montr-core/components";
import { RouteComponentProps } from "react-router-dom";
import { Translation } from "react-i18next";
import { AccountService } from "../services/account-service";

interface RouteProps {
	userId: string;
	email: string;
	code: string;
}

interface Props extends RouteComponentProps<RouteProps> {
}

interface State {
	loading: boolean;
}

export default class ConfirmEmailChange extends React.Component<Props, State> {

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
		const { userId, email, code } = this.props.match.params;

		const result = await this._accountService.confirmEmailChange({ userId, email, code });

		if (result.success) {
			this.setState({ loading: false });
		}
	};

	handleContinue = async () => {
		this.props.history.push(Patterns.dashboard);
	};

	render = () => {
		const { loading } = this.state;

		return (
			<Translation ns="idx">
				{(t) => <Page title={t("page.confirmEmailChange.title")}>
					<Spin spinning={loading}>

						{!loading && <p>Thank you for confirming your email change.</p>}

						<Button disabled={loading} onClick={this.handleContinue}>{t("button.continue")}</Button>
					</Spin>
				</Page>}
			</Translation>
		);
	};
}
