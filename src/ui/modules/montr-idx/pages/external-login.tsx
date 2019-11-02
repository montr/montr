import * as React from "react";
import { Page } from "@montr-core/components";
import { Spin } from "antd";
import { Translation } from "react-i18next";
import { AccountService } from "../services/account-service";
import { IExternalLoginModel } from "../models";

interface IProps {
}

interface IState {
	loading: boolean;
}

export default class ExternalLogin extends React.Component<IProps, IState> {

	private _accountService = new AccountService();

	constructor(props: IProps) {
		super(props);

		this.state = {
			loading: true
		};
	}

	componentDidMount = async () => {
		await this.fetchData();
	}

	componentWillUnmount = async () => {
		await this._accountService.abort();
	}

	fetchData = async () => {

		const params = new URLSearchParams(window.location.search);

		const request: IExternalLoginModel = {
			returnUrl: params.get("returnUrl"),
			remoteError: params.get("remoteError")
		};

		const result = await this._accountService.externalLoginCallback(request);

		if (result.success) {
			this.setState({ loading: false });

			window.location.href = result.redirectUrl;
		}
	}

	render = () => {
		const { loading } = this.state;

		return (
			<Translation ns="idx">
				{(t) => <Page title={t("page.externalLogin.title")}>
					<Spin spinning={loading}>


					</Spin>
				</Page>}
			</Translation>
		);
	}
}
