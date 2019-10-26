import { Fetcher } from "@montr-core/services/fetcher";
import { Constants } from "@montr-core/constants";
import { IApiResult } from "@montr-core/models";
import { IConfirmEmailModel, IRegisterUserModel } from "../models";

export class AccountService extends Fetcher {

	register = async (request: IRegisterUserModel): Promise<IApiResult> => {
		return this.post(`${Constants.apiURL}/account/register`, request);
	};

	confirmEmail = async (request: IConfirmEmailModel): Promise<IApiResult> => {
		return this.post(`${Constants.apiURL}/account/confirmEmail`, request);
	};

}
