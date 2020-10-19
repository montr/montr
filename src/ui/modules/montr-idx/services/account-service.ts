import { Fetcher } from "@montr-core/services/fetcher";
import { Constants } from "@montr-core/constants";
import { ApiResult } from "@montr-core/models";
import { IConfirmEmailModel, IRegisterModel, ILoginModel, IResetPasswordModel, IForgotPasswordModel, IAuthScheme, IExternalLoginModel, IExternalRegisterModel, ISendEmailConfirmationModel, IConfirmEmailChangeModel } from "../models/";

export class AccountService extends Fetcher {

	register = async (request: IRegisterModel): Promise<ApiResult> => {
		return this.post(`${Constants.apiURL}/account/register`, request);
	};

	sendEmailConfirmation = async (request: ISendEmailConfirmationModel): Promise<ApiResult> => {
		return this.post(`${Constants.apiURL}/account/sendEmailConfirmation`, request);
	};

	confirmEmail = async (request: IConfirmEmailModel): Promise<ApiResult> => {
		return this.post(`${Constants.apiURL}/account/confirmEmail`, request);
	};

	confirmEmailChange = async (request: IConfirmEmailChangeModel): Promise<ApiResult> => {
		return this.post(`${Constants.apiURL}/account/confirmEmailChange`, request);
	};

	login = async (request: ILoginModel): Promise<ApiResult> => {
		return this.post(`${Constants.apiURL}/account/login`, request);
	};

	externalLoginCallback = async (request: IExternalLoginModel): Promise<ApiResult<IExternalRegisterModel>> => {
		return this.post(`${Constants.apiURL}/account/externalLoginCallback`, request);
	};

	externalRegister = async (request: IExternalRegisterModel): Promise<ApiResult> => {
		return this.post(`${Constants.apiURL}/account/externalRegister`, request);
	};

	authSchemes = async (): Promise<IAuthScheme[]> => {
		return this.post(`${Constants.apiURL}/account/authSchemes`, {});
	};

	logout = async (): Promise<ApiResult> => {
		return this.post(`${Constants.apiURL}/account/logout`, {});
	};

	forgotPassword = async (request: IForgotPasswordModel): Promise<ApiResult> => {
		return this.post(`${Constants.apiURL}/account/forgotPassword`, request);
	};

	resetPassword = async (request: IResetPasswordModel): Promise<ApiResult> => {
		return this.post(`${Constants.apiURL}/account/resetPassword`, request);
	};

}
