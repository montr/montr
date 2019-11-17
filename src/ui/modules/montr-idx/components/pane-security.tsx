import React from "react";
import { Translation } from "react-i18next";
import { Button, List } from "antd";
import { PageHeader } from "@montr-core/components";
import { IProfileModel } from "../models";
import { ModalChangePassword, ModalSetPassword } from "./";
import { ProfileService } from "../services";

interface IProps {
}

interface IState {
	loading: boolean;
	data: IProfileModel;
	displayPasswordModal: "changePassword" | "setPassword" | boolean;
}

export class PaneSecurity extends React.Component<IProps, IState> {

	private _profileService = new ProfileService();

	constructor(props: IProps) {
		super(props);

		this.state = {
			loading: true,
			data: {},
			displayPasswordModal: false
		};
	}

	componentDidMount = async () => {
		await this.fetchData();
	};

	fetchData = async () => {
		const data = await this._profileService.get();

		this.setState({ loading: false, data });
	};

	handleChangePassword = () => {
		this.setState({ displayPasswordModal: "changePassword" });
	};

	handleSetPassword = () => {
		this.setState({ displayPasswordModal: "setPassword" });
	};

	handlePasswordModalSuccess = async () => {
		this.setState({ displayPasswordModal: false });

		await this.fetchData();
	};

	handlePasswordModalCancel = () => {
		this.setState({ displayPasswordModal: false });
	};

	render = () => {
		const { displayPasswordModal } = this.state;

		return (
			<Translation ns="idx">
				{(t) => <>
					<PageHeader>{t("page.security.title")}</PageHeader>
					<h3>{t("page.security.subtitle")}</h3>

					{displayPasswordModal == "changePassword" &&
						<ModalChangePassword
							onSuccess={this.handlePasswordModalSuccess}
							onCancel={this.handlePasswordModalCancel}
						/>}

					{displayPasswordModal == "setPassword" &&
						<ModalSetPassword
							onSuccess={this.handlePasswordModalSuccess}
							onCancel={this.handlePasswordModalCancel}
						/>}

					{/*  todo: create DataList component */}
					<List>
						<List.Item actions={[
							<Button onClick={this.handleChangePassword}>{t("button.changePassword")}</Button>
						]}>
							<List.Item.Meta
								title="Password"
								description="Last changed Feb 21, 2017"
							/>
						</List.Item>
						<List.Item actions={[
							<Button onClick={this.handleSetPassword}>{t("button.setPassword")}</Button>
						]}>
							<List.Item.Meta
								title="Password"
								description="Account does not have a password"
							/>
						</List.Item>
						<List.Item actions={[<Button key="1">Change Email</Button>]}>
							<List.Item.Meta
								title="Email"
								description="Last changed Feb 21, 2017"
							/>
						</List.Item>
						<List.Item actions={[<Button key="1">Change Phone</Button>]}>
							<List.Item.Meta
								title="Phone"
								description="Last changed Feb 21, 2017"
							/>
						</List.Item>
					</List>
				</>}
			</Translation>
		);
	};
}
