import { Fetcher } from "./fetcher";
import { Constants } from "../constants";
import { IApiResult, IConfirmEmailModel, IRegisterUserModel } from "../models";

export class AccountService extends Fetcher {

	register = async (request: IRegisterUserModel): Promise<IApiResult> => {
		return this.post(`${Constants.apiURL}/account/register`, request);
	};

	confirmEmail = async (request: IConfirmEmailModel): Promise<IApiResult> => {
		return this.post(`${Constants.apiURL}/account/confirmEmail`, request);
	};

}
