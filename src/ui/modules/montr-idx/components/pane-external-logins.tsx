import React from "react";
import { Translation } from "react-i18next";
import { PageHeader } from "@montr-core/components";
import { NavigationService } from "@montr-core/services";
import { IProfileModel, IUserLoginInfo, IAuthScheme } from "../models";
import { ProfileService, AccountService } from "../services";
import { Spin, List, Button, Avatar } from "antd";
import { Constants } from "@montr-core/constants";

interface IProps {
}

interface IState {
	loading: boolean;
	profile: IProfileModel;
	authSchemes: IAuthScheme[];
	externalLogins: IUserLoginInfo[];
}

export default class PaneExternalLogins extends React.Component<IProps, IState> {

	private _navigation = new NavigationService();
	private _accountService = new AccountService();
	private _profileService = new ProfileService();

	constructor(props: IProps) {
		super(props);

		this.state = {
			loading: true,
			profile: {},
			authSchemes: [],
			externalLogins: []
		};
	}

	componentDidMount = async () => {
		await this.fetchData();
	};

	componentWillUnmount = async () => {
		await this._accountService.abort();
		await this._profileService.abort();
	};

	fetchData = async () => {
		const profile = await this._profileService.get();

		const authSchemes = await this._accountService.authSchemes();
		const externalLogins = await this._profileService.externalLogins();

		this.setState({ loading: false, profile, authSchemes, externalLogins });
	};

	handleRemoveLogin = async (info: IUserLoginInfo) => {
		const result = await this._profileService.removeLogin(info);

		if (result.success) {
			await this.fetchData();
		}
	};

	render = () => {
		const { loading, profile, authSchemes, externalLogins } = this.state;

		const otherLogins = authSchemes
			.filter(s => externalLogins.findIndex(x => s.name == x.loginProvider) === -1),
			showRemoveButton = profile.hasPassword || externalLogins.length > 0;

		return (
			<Translation ns="idx">
				{(t) => <>

					<PageHeader>{t("page.externalLogins.title")}</PageHeader>
					<h3>{t("page.externalLogins.subtitle")}</h3>

					<Spin spinning={loading}>
						<form method="post" action={`${Constants.apiURL}/authentication/linkLogin`}>

							{/* <input type="hidden" name={Constants.returnUrlParam} value={this._navigation.getReturnUrlParameter() || ""} /> */}

							<List className="external-logins">

								{externalLogins.map((x) => {
									const scheme = authSchemes.find(s => s.name == x.loginProvider);
									return (
										<List.Item
											key={x.loginProvider}
											actions={
												showRemoveButton ? [
													<Button
														onClick={() => this.handleRemoveLogin(x)}>
														{t("button.removeLogin")}
													</Button>
												] : null}>
											<List.Item.Meta
												avatar={<Avatar icon={scheme.icon} className={scheme.icon} />}
												title={x.providerDisplayName}
												description="Last used Feb 21, 2017"
											/>
										</List.Item>
									);
								})}

								{otherLogins.map((x) => {
									return (
										<List.Item
											key={x.name}
											actions={[
												<Button
													htmlType="submit"
													name="provider"
													value={x.name}>
													{t("button.addLogin")}
												</Button>
											]}>
											<List.Item.Meta
												avatar={<Avatar icon={x.icon} className={x.icon} />}
												title={x.displayName}
												description="Last used Feb 21, 2017"
											/>
										</List.Item>
									);
								})}

							</List>

						</form>
					</Spin>

				</>}
			</Translation>
		);
	};
}
