import { Fetcher } from "@montr-core/services";
import { Constants } from "@montr-core/constants";
import { IApiResult } from "@montr-core/models";
import { IProfileModel, IChangePasswordModel, ISetPasswordModel } from "../models";

export class ProfileService extends Fetcher {

	get = async (): Promise<IProfileModel> => {
		return this.post(`${Constants.apiURL}/profile/get`);
	};

	update = async (request: IProfileModel): Promise<IApiResult> => {
		return this.post(`${Constants.apiURL}/profile/update`, request);
	};

	changePassword = async (request: IChangePasswordModel): Promise<IApiResult> => {
		return this.post(`${Constants.apiURL}/profile/changePassword`, request);
	};

	setPassword = async (request: ISetPasswordModel): Promise<IApiResult> => {
		return this.post(`${Constants.apiURL}/profile/setPassword`, request);
	};

}
