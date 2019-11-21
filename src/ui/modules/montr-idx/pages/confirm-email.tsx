import * as React from "react";
import { Page } from "@montr-core/components";
import { Spin, Button } from "antd";
import { RouteComponentProps } from "react-router-dom";
import { Translation } from "react-i18next";
import { AccountService } from "../services/account-service";

interface IRouteProps {
	userId: string;
	code: string;
}

interface IProps extends RouteComponentProps<IRouteProps> {
}

interface IState {
	loading: boolean;
}

export default class ConfirmEmail extends React.Component<IProps, IState> {

	private _accountService = new AccountService();

	constructor(props: IProps) {
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
		// todo: use route const
		this.props.history.push("/dashboard");
	};

	render = () => {
		const { loading } = this.state;

		return (
			<Translation ns="idx">
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
