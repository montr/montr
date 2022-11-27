import { Icon, PageHeader } from "@montr-core/components";
import { OperationService } from "@montr-core/services/operation-service";
import { Button, List, Tooltip } from "antd";
import React from "react";
import { Translation } from "react-i18next";
import { ProfileModel } from "../models/profile-model";
import { Locale } from "../module";
import { AccountService } from "../services/account-service";
import { ProfileService } from "../services/profile-service";
import { ModalChangeEmail, ModalChangePassword, ModalChangePhone, ModalSetPassword } from "./";

declare const ModalTypes: ["changePassword", "setPassword", "changeEmail", "changePhone"];

interface Props {
}

interface State {
	loading: boolean;
	data: ProfileModel;
	displayModal: (typeof ModalTypes)[number] | boolean;
	sendEmailConfirmationDisabled: boolean;
	sendPhoneConfirmationDisabled: boolean;
}

export default class PaneSecurity extends React.Component<Props, State> {

	private _operation = new OperationService();
	private _accountService = new AccountService();
	private _profileService = new ProfileService();

	constructor(props: Props) {
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

		await this._operation.execute(async () => {
			return await this._accountService.sendEmailConfirmation({});
		});

		this.setState({ sendEmailConfirmationDisabled: false });
	};

	render = () => {
		const { data, displayModal, sendEmailConfirmationDisabled, sendPhoneConfirmationDisabled } = this.state;

		const confirmed = <Tooltip title="Confirmed">{Icon.CheckCircleTwoTone}</Tooltip>,
			notConfirmed = <Tooltip title="Not confirmed">{Icon.QuestionCircle}</Tooltip>;

		return (
			<Translation ns={Locale.Namespace}>
				{(t) => <>
					<PageHeader>{t("page.security.title") as string}</PageHeader>
					<h3>{t("page.security.subtitle") as string}</h3>

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
								<Button onClick={() => this.handleDisplayModal("changePassword")}>{t("button.changePassword") as string}</Button>]}>
								<List.Item.Meta
									title="Password"
									description="Last changed Feb 21, 2017" // todo: add real dates
								/>
							</List.Item>
						}

						{data.hasPassword == false &&
							<List.Item actions={[
								<Button onClick={() => this.handleDisplayModal("setPassword")}>{t("button.setPassword") as string}</Button>]}>
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
								onClick={() => this.sendEmailConfirmation()}>{t("button.sendVerificationEmail") as string}</Button>,
							<Button onClick={() => this.handleDisplayModal("changeEmail")}>{t("button.changeEmail") as string}</Button>
						].filter(x => x)}>
							<List.Item.Meta
								title="Email"
								description={<>{data.email} {data.isEmailConfirmed ? confirmed : notConfirmed} </>}
							/>
						</List.Item>

						<List.Item actions={[
							!data.isPhoneNumberConfirmed &&
							<Button type="link"
								disabled={sendPhoneConfirmationDisabled}>{t("button.sendVerificationSms") as string}</Button>,
							<Button onClick={() => this.handleDisplayModal("changePhone")}>{t("button.changePhone") as string}</Button>
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
