import { Icon, PageHeader } from "@montr-core/components";
import { OperationService } from "@montr-core/services/operation-service";
import { Avatar, Button, List, Spin } from "antd";
import React from "react";
import { Translation } from "react-i18next";
import { AuthScheme, ProfileModel, UserLoginInfo } from "../models";
import { Api, Locale } from "../module";
import { AccountService } from "../services/account-service";
import { ProfileService } from "../services/profile-service";

interface State {
	loading: boolean;
	profile: Partial<ProfileModel>;
	authSchemes: AuthScheme[];
	externalLogins: UserLoginInfo[];
}

export default class PaneExternalLogins extends React.Component<unknown, State> {

	private readonly operation = new OperationService();
	private readonly accountService = new AccountService();
	private readonly profileService = new ProfileService();

	constructor(props: unknown) {
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
		await this.accountService.abort();
		await this.profileService.abort();
	};

	fetchData = async () => {
		const profile = await this.profileService.get();

		const authSchemes = await this.accountService.authSchemes();
		const externalLogins = await this.profileService.externalLogins();

		this.setState({ loading: false, profile, authSchemes, externalLogins });
	};

	handleRemoveLogin = async (info: UserLoginInfo) => {
		await this.operation.execute(async () => {
			const result = await this.profileService.removeLogin(info);
			if (result.success) {
				await this.fetchData();
			}
			return result;
		});
	};

	render = () => {
		const { loading, profile, authSchemes, externalLogins } = this.state;

		const otherLogins = authSchemes
			.filter(s => externalLogins.findIndex(x => s.name == x.loginProvider) === -1),
			showRemoveButton = profile.hasPassword || externalLogins.length > 0;

		return (
			<Translation ns={Locale.Namespace}>
				{(t) => <>

					<PageHeader>{t("page.externalLogins.title") as string}</PageHeader>
					<p>{t("page.externalLogins.subtitle") as string}</p>

					<Spin spinning={loading}>
						<form method="post" action={Api.authLinkLogin} className="external-logins">

							{/* <input type="hidden" name={Constants.returnUrlParam} value={this._navigation.getReturnUrlParameter() || ""} /> */}

							<List>

								{externalLogins.map((x) => {
									const scheme = authSchemes.find(s => s.name == x.loginProvider);
									return (
										<List.Item
											key={x.loginProvider}
											actions={
												showRemoveButton ? [
													<Button
														onClick={() => this.handleRemoveLogin(x)}>
														{t("button.removeLogin") as string}
													</Button>
												] : null}>
											<List.Item.Meta
												avatar={<Avatar icon={Icon.get(scheme.icon)} className={scheme.icon} />}
												title={x.providerDisplayName}
												description="Last used Feb 21, 2017" // todo: add real dates
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
													{t("button.addLogin") as string}
												</Button>
											]}>
											<List.Item.Meta
												avatar={<Avatar icon={Icon.get(x.icon)} className={x.icon} />}
												title={x.displayName}
												description="Last used Feb 21, 2017" // todo: add real dates
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
