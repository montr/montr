import React from "react";
import { Translation } from "react-i18next";
import { Button, List, Icon } from "antd";
import { PageHeader } from "@montr-core/components";
import { IProfileModel } from "../models";
import { ModalChangePassword, ModalSetPassword } from "./";
import { ProfileService } from "../services";
import { ModalChangePhone } from "./modal-change-phone";

declare const ModalTypes: ["changePassword", "setPassword", "changeEmail", "changePhone"];

interface IProps {
}

interface IState {
	loading: boolean;
	data: IProfileModel;
	displayModal: (typeof ModalTypes)[number] | boolean;
}

export class PaneSecurity extends React.Component<IProps, IState> {

	private _profileService = new ProfileService();

	constructor(props: IProps) {
		super(props);

		this.state = {
			loading: true,
			data: {},
			displayModal: false
		};
	}

	componentDidMount = async () => {
		await this.fetchData();
	};

	fetchData = async () => {
		const data = await this._profileService.get();

		this.setState({ loading: false, data });
	};

	handleDisplayModal = (modalType: (typeof ModalTypes)[number]) => {
		this.setState({ displayModal: modalType });
	};

	handleModalSuccess = async () => {
		this.setState({ displayModal: false });

		await this.fetchData();
	};

	handleModalCancel = () => {
		this.setState({ displayModal: false });
	};

	render = () => {
		const { data, displayModal } = this.state;

		return (
			<Translation ns="idx">
				{(t) => <>
					<PageHeader>{t("page.security.title")}</PageHeader>
					<h3>{t("page.security.subtitle")}</h3>

					{displayModal == "changePassword" &&
						<ModalChangePassword
							onSuccess={this.handleModalSuccess}
							onCancel={this.handleModalCancel}
						/>}

					{displayModal == "setPassword" &&
						<ModalSetPassword
							onSuccess={this.handleModalSuccess}
							onCancel={this.handleModalCancel}
						/>}

					{displayModal == "changePhone" &&
						<ModalChangePhone
							onSuccess={this.handleModalSuccess}
							onCancel={this.handleModalCancel}
						/>}

					{/*  todo: create DataList component */}
					<List>
						{data.hasPassword &&
							<List.Item actions={[
								<Button onClick={() => this.handleDisplayModal("changePassword")}>{t("button.changePassword")}</Button>]}>
								<List.Item.Meta
									title="Password"
									description="Last changed Feb 21, 2017"
								/>
							</List.Item>
						}

						{data.hasPassword == false &&
							<List.Item actions={[
								<Button onClick={() => this.handleDisplayModal("setPassword")}>{t("button.setPassword")}</Button>]}>
								<List.Item.Meta
									title="Password"
									description="Account does not have a password"
								/>
							</List.Item>
						}

						<List.Item actions={[
							<Button onClick={() => this.handleDisplayModal("changeEmail")}>{t("button.changeEmail")}</Button>]}>
							<List.Item.Meta
								title="Email"
								description={data.userName}
							/>
							{data.isEmailConfirmed
								? <><Icon type="check" /> &#xA0; Email confirmed</>
								: <a>Send verification email</a>}
						</List.Item>

						<List.Item actions={[
							<Button onClick={() => this.handleDisplayModal("changePhone")}>{t("button.changePhone")}</Button>]}>
							<List.Item.Meta
								title="Phone"
								description={data.phoneNumber}
							/>
							{data.isPhoneNumberConfirmed
								? <><Icon type="check" /> &#xA0; Phone confirmed</>
								: <a>Send verification SMS</a>}
						</List.Item>
					</List>
				</>}
			</Translation>
		);
	};
}
