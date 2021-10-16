import { ApiResult } from "@montr-core/models";
import { Fetcher } from "@montr-core/services/fetcher";
import { AuthScheme, ConfirmEmailChangeModel, ConfirmEmailModel, ExternalLoginModel, ExternalRegisterModel, ForgotPasswordModel, LoginModel, RegisterModel, ResetPasswordModel, SendEmailConfirmationModel } from "../models/";
import { Api } from "../module";

export class AccountService extends Fetcher {

	register = async (request: RegisterModel): Promise<ApiResult> => {
		return this.post(Api.accountRegister, request);
	};

	sendEmailConfirmation = async (request: SendEmailConfirmationModel): Promise<ApiResult> => {
		return this.post(Api.accountSendEmailConfirmation, request);
	};

	confirmEmail = async (request: ConfirmEmailModel): Promise<ApiResult> => {
		return this.post(Api.accountConfirmEmail, request);
	};

	confirmEmailChange = async (request: ConfirmEmailChangeModel): Promise<ApiResult> => {
		return this.post(Api.accountConfirmEmailChange, request);
	};

	login = async (request: LoginModel): Promise<ApiResult> => {
		return this.post(Api.accountLogin, request);
	};

	externalLoginCallback = async (request: ExternalLoginModel): Promise<ApiResult<ExternalRegisterModel>> => {
		return this.post(Api.accountExternalLoginCallback, request);
	};

	externalRegister = async (request: ExternalRegisterModel): Promise<ApiResult> => {
		return this.post(Api.accountExternalRegister, request);
	};

	authSchemes = async (): Promise<AuthScheme[]> => {
		return this.post(Api.accountAuthSchemes, {});
	};

	logout = async (): Promise<ApiResult> => {
		return this.post(Api.accountLogout, {});
	};

	forgotPassword = async (request: ForgotPasswordModel): Promise<ApiResult> => {
		return this.post(Api.accountForgotPassword, request);
	};

	resetPassword = async (request: ResetPasswordModel): Promise<ApiResult> => {
		return this.post(Api.accountResetPassword, request);
	};

}
