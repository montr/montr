import { Fetcher } from "@montr-core/services/fetcher";
import { Constants } from "@montr-core/constants";
import { ApiResult } from "@montr-core/models";
import { ConfirmEmailModel, RegisterModel, LoginModel, ResetPasswordModel, ForgotPasswordModel, AuthScheme, ExternalLoginModel, ExternalRegisterModel, SendEmailConfirmationModel, ConfirmEmailChangeModel } from "../models/";

export class AccountService extends Fetcher {

	register = async (request: RegisterModel): Promise<ApiResult> => {
		return this.post(`${Constants.apiURL}/account/register`, request);
	};

	sendEmailConfirmation = async (request: SendEmailConfirmationModel): Promise<ApiResult> => {
		return this.post(`${Constants.apiURL}/account/sendEmailConfirmation`, request);
	};

	confirmEmail = async (request: ConfirmEmailModel): Promise<ApiResult> => {
		return this.post(`${Constants.apiURL}/account/confirmEmail`, request);
	};

	confirmEmailChange = async (request: ConfirmEmailChangeModel): Promise<ApiResult> => {
		return this.post(`${Constants.apiURL}/account/confirmEmailChange`, request);
	};

	login = async (request: LoginModel): Promise<ApiResult> => {
		return this.post(`${Constants.apiURL}/account/login`, request);
	};

	externalLoginCallback = async (request: ExternalLoginModel): Promise<ApiResult<ExternalRegisterModel>> => {
		return this.post(`${Constants.apiURL}/account/externalLoginCallback`, request);
	};

	externalRegister = async (request: ExternalRegisterModel): Promise<ApiResult> => {
		return this.post(`${Constants.apiURL}/account/externalRegister`, request);
	};

	authSchemes = async (): Promise<AuthScheme[]> => {
		return this.post(`${Constants.apiURL}/account/authSchemes`, {});
	};

	logout = async (): Promise<ApiResult> => {
		return this.post(`${Constants.apiURL}/account/logout`, {});
	};

	forgotPassword = async (request: ForgotPasswordModel): Promise<ApiResult> => {
		return this.post(`${Constants.apiURL}/account/forgotPassword`, request);
	};

	resetPassword = async (request: ResetPasswordModel): Promise<ApiResult> => {
		return this.post(`${Constants.apiURL}/account/resetPassword`, request);
	};

}
