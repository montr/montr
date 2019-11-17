import React from "react";
import { Translation } from "react-i18next";
import { PageHeader } from "@montr-core/components";
import { IProfileModel, IUserLoginInfo, IAuthScheme } from "../models";
import { ProfileService, AccountService } from "../services";
import { Spin, List, Button, Icon, Avatar } from "antd";

interface IProps {
}

interface IState {
	loading: boolean;
	profile: IProfileModel;
	authSchemes: IAuthScheme[];
	externalLogins: IUserLoginInfo[];
}

export class PaneExternalLogins extends React.Component<IProps, IState> {

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
						<List>

							{externalLogins.map((x) => {

								const scheme = authSchemes.find(s => s.name == x.loginProvider);

								const actions = showRemoveButton
									? [<Button key="1">Remove</Button>] : null;

								return (
									<List.Item
										key={x.loginProvider}
										actions={actions}>
										<List.Item.Meta
											/* todo: use icon */
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
										actions={[<Button key="1">Add</Button>]}>
										<List.Item.Meta
											avatar={<Avatar icon={x.icon} className={x.icon} />}
											title={x.displayName}
											description="Last used Feb 21, 2017"
										/>
									</List.Item>
								);
							})}


						</List>
					</Spin>
				</>}
			</Translation>
		);
	};
}
