import React from "react";
import { IAuthScheme } from "../models";
import { Button, Spin } from "antd";
import { NavigationService } from "@montr-core/services";
import { AccountService } from "../services/account-service";
import { Constants } from "@montr-core/constants";

interface IProps {
}

interface IState {
	loading: boolean;
	authSchemes: IAuthScheme[];
}

export class ExternalLoginForm extends React.Component<IProps, IState> {

	private _navigation = new NavigationService();
	private _accountService = new AccountService();

	constructor(props: IProps) {
		super(props);

		this.state = {
			loading: true,
			authSchemes: []
		};
	}

	componentDidMount = async () => {
		await this.fetchData();
	}

	componentWillUnmount = async () => {
		await this._accountService.abort();
	}

	fetchData = async () => {
		const authSchemes = await this._accountService.authSchemes();

		this.setState({ loading: false, authSchemes });
	}

	render = () => {
		const { loading, authSchemes } = this.state;

		return (
			<Spin spinning={loading}>

				{authSchemes.length == 0 && <p>
					There are no external authentication services configured.
				</p>}

				{authSchemes.length > 0 && <form method="post" action={`${Constants.apiURL}/authentication/externalLogin`}>
					<input type="hidden" name={Constants.returnUrlParam} value={this._navigation.getReturnUrlParameter() || ""} />
					{authSchemes.map(x => (
						<Button
							key={x.name}
							htmlType="submit"
							name="provider"
							value={x.name}
							icon={x.name.toLowerCase()}>
							{`Log in using your ${x.displayName} account`}
						</Button>
					))}
				</form>}
			</Spin>
		);
	}
}
