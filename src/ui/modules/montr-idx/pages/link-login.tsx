import React from "react";
import { RouteComponentProps } from "react-router";
import { Spin } from "antd";
import { Translation } from "react-i18next";
import { Page } from "@montr-core/components";
import { NavigationService, NotificationService } from "@montr-core/services";
import { ProfileService } from "../services";
import { Patterns } from "@montr-idx/module";

interface IProps extends RouteComponentProps {
}

interface IState {
	loading: boolean;
	// data?: IExternalRegisterModel;
	// fields?: IFormField[];
}

export default class LinkLogin extends React.Component<IProps, IState> {

	private _navigation = new NavigationService();
	private _notification = new NotificationService();
	private _profileService = new ProfileService();

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
		await this._profileService.abort();
	};

	fetchData = async () => {

		const result = await this._profileService.linkLoginCallback();

		this.setState({ loading: false });

		if (result.success) {
			this.props.history.push(Patterns.profileExternalLogin);
		}
		else {
			if (result.errors) {
				this._notification.error(result.errors[0].messages);
			}
		}
	};

	render = () => {
		const { loading } = this.state;

		return (
			<Translation ns="idx">
				{(t) => <Page title={t("page.linkLogin.title")}>
					<Spin spinning={loading}>

					</Spin>
				</Page>}
			</Translation>
		);
	};
}
