import { Icon } from "@montr-core/components";
import { Constants } from "@montr-core/constants";
import { NavigationService } from "@montr-core/services";
import { Avatar, Button, Spin } from "antd";
import React from "react";
import { Translation } from "react-i18next";
import { AuthScheme } from "../models";
import { Api, Locale } from "../module";
import { AccountService } from "../services/account-service";

interface State {
	loading: boolean;
	authSchemes: AuthScheme[];
}

export class ExternalLoginForm extends React.Component<unknown, State> {

	private readonly navigation = new NavigationService();
	private readonly accountService = new AccountService();

	constructor(props: unknown) {
		super(props);

		this.state = {
			loading: true,
			authSchemes: []
		};
	}

	componentDidMount = async () => {
		await this.fetchData();
	};

	componentWillUnmount = async () => {
		await this.accountService.abort();
	};

	fetchData = async () => {
		const authSchemes = await this.accountService.authSchemes();

		this.setState({ loading: false, authSchemes });
	};

	render = () => {
		const { loading, authSchemes } = this.state;

		return (
			<Translation ns={Locale.Namespace}>
				{(t) => <Spin spinning={loading}>

					{/* {authSchemes.length == 3 && <p>{t("page.login.noExternalLogins")}</p>} */}

					{authSchemes.length > 0 && <form method="post" action={Api.authExternalLogin} className="external-logins">
						<input type="hidden" name={Constants.returnUrlParam} value={this.navigation.getReturnUrlParameter() || ""} />
						{authSchemes.map(x => (
							<Button
								title={t("button.externalLogin.title", { provider: x.displayName })}
								style={{ padding: 0, margin: "0 8px 8px 0" }}
								type="link"
								key={x.name}
								htmlType="submit"
								name="provider"
								value={x.name}>
								<Avatar icon={Icon.get(x.icon)} className={x.icon} />
							</Button>
						))}
					</form>}
				</Spin>}
			</Translation>
		);
	};
}
