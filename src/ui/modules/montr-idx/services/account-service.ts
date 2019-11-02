import { Fetcher } from "@montr-core/services/fetcher";
import { Constants } from "@montr-core/constants";
import { IApiResult } from "@montr-core/models";
import { IConfirmEmailModel, IRegisterUserModel, ILoginModel, IResetPasswordModel, IForgotPasswordModel, IAuthScheme, IExternalLoginModel } from "../models/";

export class AccountService extends Fetcher {

	register = async (request: IRegisterUserModel): Promise<IApiResult> => {
		return this.post(`${Constants.apiURL}/account/register`, request);
	};

	sendEmailConfirmation = async (request: ILoginModel): Promise<IApiResult> => {
		return this.post(`${Constants.apiURL}/account/sendEmailConfirmation`, request);
	};

	confirmEmail = async (request: IConfirmEmailModel): Promise<IApiResult> => {
		return this.post(`${Constants.apiURL}/account/confirmEmail`, request);
	};

	login = async (request: ILoginModel): Promise<IApiResult> => {
		return this.post(`${Constants.apiURL}/account/login`, request);
	};

	externalLoginCallback = async (request: IExternalLoginModel): Promise<IApiResult> => {
		return this.post(`${Constants.apiURL}/account/externalLoginCallback`, request);
	};

	authSchemes = async (): Promise<IAuthScheme[]> => {
		return this.post(`${Constants.apiURL}/account/authSchemes`, {});
	};

	logout = async (): Promise<IApiResult> => {
		return this.post(`${Constants.apiURL}/account/logout`, {});
	};

	forgotPassword = async (request: IForgotPasswordModel): Promise<IApiResult> => {
		return this.post(`${Constants.apiURL}/account/forgotPassword`, request);
	};

	resetPassword = async (request: IResetPasswordModel): Promise<IApiResult> => {
		return this.post(`${Constants.apiURL}/account/resetPassword`, request);
	};

}
