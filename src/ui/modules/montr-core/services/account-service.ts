import { Fetcher } from "./fetcher";
import { Constants } from "../constants";
import { IApiResult } from "../models";
import { IRegisterUserModel } from "@montr-core/models/register-user-model";

interface IRegisterUserRequest {
	locale: string;
	module: string;
}

export class AccountService extends Fetcher {
	register = async (request: IRegisterUserModel): Promise<IApiResult> => {
		return this.post(`${Constants.apiURL}/account/register`, request);
	};
}
