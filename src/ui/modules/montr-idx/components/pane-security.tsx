import React from "react";
import { Translation } from "react-i18next";
import { Button, List, Tooltip } from "antd";
import { PageHeader, Icon } from "@montr-core/components";
import { OperationService } from "@montr-core/services";
import { IProfileModel } from "../models";
import { ModalChangePassword, ModalSetPassword, ModalChangeEmail } from "./";
import { ProfileService, AccountService } from "../services";
import { ModalChangePhone } from "./modal-change-phone";

declare const ModalTypes: ["changePassword", "setPassword", "changeEmail", "changePhone"];

interface IProps {
}

interface IState {
	loading: boolean;
	data: IProfileModel;
	displayModal: (typeof ModalTypes)[number] | boolean;
	sendEmailConfirmationDisabled: boolean;
	sendPhoneConfirmationDisabled: boolean;
}

export default class PaneSecurity extends React.Component<IProps, IState> {

	private _operation = new OperationService();
	private _accountService = new AccountService();
	private _profileService = new ProfileService();

	constructor(props: IProps) {
		super(props);

		this.state = {
			loading: true,
			data: {},
			displayModal: false,
			sendEmailConfirmationDisabled: false,
			sendPhoneConfirmationDisabled: true
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

	sendEmailConfirmation = async () => {
		this.setState({ sendEmailConfirmationDisabled: true });

		await this._operation.execute(() => this._accountService.sendEmailConfirmation({}));

		this.setState({ sendEmailConfirmationDisabled: false });
	};

	render = () => {
		const { data, displayModal, sendEmailConfirmationDisabled, sendPhoneConfirmationDisabled } = this.state;

		const confirmed = <Tooltip title="Confirmed">{Icon.CheckCircleTwoTone}</Tooltip>,
			notConfirmed = <Tooltip title="Not confirmed">{Icon.QuestionCircle}</Tooltip>;

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

					{displayModal == "changeEmail" &&
						<ModalChangeEmail
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
									description="Last changed Feb 21, 2017" // todo: add real dates
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
							!data.isEmailConfirmed &&
							<Button type="link"
								disabled={sendEmailConfirmationDisabled}
								onClick={() => this.sendEmailConfirmation()}>{t("button.sendVerificationEmail")}</Button>,
							<Button onClick={() => this.handleDisplayModal("changeEmail")}>{t("button.changeEmail")}</Button>
						].filter(x => x)}>
							<List.Item.Meta
								title="Email"
								description={<>{data.email} {data.isEmailConfirmed ? confirmed : notConfirmed} </>}
							/>
						</List.Item>

						<List.Item actions={[
							!data.isPhoneNumberConfirmed &&
							<Button type="link"
								disabled={sendPhoneConfirmationDisabled}>{t("button.sendVerificationSms")}</Button>,
							<Button onClick={() => this.handleDisplayModal("changePhone")}>{t("button.changePhone")}</Button>
						].filter(x => x)}>
							<List.Item.Meta
								title="Phone"
								description={<>{data.phoneNumber} {data.isPhoneNumberConfirmed ? confirmed : notConfirmed} </>}
							/>
						</List.Item>
					</List>
				</>}
			</Translation>
		);
	};
}
