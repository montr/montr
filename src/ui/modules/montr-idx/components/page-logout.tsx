import * as React from "react";
import { Page } from "@montr-core/components";
import { Spin } from "antd";
import { Translation } from "react-i18next";
import { AccountService } from "../services/account-service";

interface Props {
}

interface State {
	loading: boolean;
}

export default class Logout extends React.Component<Props, State> {

	private _accountService = new AccountService();

	constructor(props: Props) {
		super(props);

		this.state = {
			loading: true
		};
	}

	componentDidMount = async () => {
		await this.logout();
	};

	componentWillUnmount = async () => {
		await this._accountService.abort();
	};

	logout = async () => {
		await this._accountService.logout();

		window.location.href = "/";

		this.setState({ loading: false });
	};

	render = () => {
		const { loading } = this.state;

		return (
			<Translation ns="idx">
				{(t) => <Page title={t("page.logout.title")}>
					<Spin spinning={loading}>
						<p>{t("page.logout.message")}</p>
					</Spin>
				</Page>}
			</Translation>
		);
	};
}
