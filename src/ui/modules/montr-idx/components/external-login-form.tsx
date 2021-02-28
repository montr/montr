import React from "react";
import { AuthScheme } from "../models";
import { Button, Spin, Avatar } from "antd";
import { NavigationService } from "@montr-core/services";
import { AccountService } from "../services/account-service";
import { Constants } from "@montr-core/constants";
import { Translation } from "react-i18next";
import { Api } from "../module";
import { Icon } from "@montr-core/components";

interface Props {
}

interface State {
	loading: boolean;
	authSchemes: AuthScheme[];
}

export class ExternalLoginForm extends React.Component<Props, State> {

	private _navigation = new NavigationService();
	private _accountService = new AccountService();

	constructor(props: Props) {
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
		await this._accountService.abort();
	};

	fetchData = async () => {
		const authSchemes = await this._accountService.authSchemes();

		this.setState({ loading: false, authSchemes });
	};

	render = () => {
		const { loading, authSchemes } = this.state;

		return (
			<Translation ns="idx">
				{(t) => <Spin spinning={loading}>

					{/* {authSchemes.length == 3 && <p>{t("page.login.noExternalLogins")}</p>} */}

					{authSchemes.length > 0 && <form method="post" action={Api.authExternalLogin} className="external-logins">
						<input type="hidden" name={Constants.returnUrlParam} value={this._navigation.getReturnUrlParameter() || ""} />
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
