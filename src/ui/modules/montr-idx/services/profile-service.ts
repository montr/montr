import { Fetcher } from "@montr-core/services";
import { Constants } from "@montr-core/constants";
import { ApiResult } from "@montr-core/models";
import { IProfileModel, IChangePasswordModel, ISetPasswordModel, IUserLoginInfo } from "../models";

export class ProfileService extends Fetcher {

	get = async (): Promise<IProfileModel> => {
		return this.post(`${Constants.apiURL}/profile/get`);
	};

	update = async (request: IProfileModel): Promise<ApiResult> => {
		return this.post(`${Constants.apiURL}/profile/update`, request);
	};

	changeEmail = async (request: IProfileModel): Promise<ApiResult> => {
		return this.post(`${Constants.apiURL}/profile/changeEmail`, request);
	};

	changePhone = async (request: IProfileModel): Promise<ApiResult> => {
		return this.post(`${Constants.apiURL}/profile/changePhone`, request);
	};

	changePassword = async (request: IChangePasswordModel): Promise<ApiResult> => {
		return this.post(`${Constants.apiURL}/profile/changePassword`, request);
	};

	setPassword = async (request: ISetPasswordModel): Promise<ApiResult> => {
		return this.post(`${Constants.apiURL}/profile/setPassword`, request);
	};

	externalLogins = async (): Promise<IUserLoginInfo[]> => {
		return this.post(`${Constants.apiURL}/profile/externalLogins`, {});
	};

	linkLoginCallback = async (): Promise<ApiResult> => {
		return this.post(`${Constants.apiURL}/profile/linkLoginCallback`, {});
	};

	removeLogin = async (request: IUserLoginInfo): Promise<ApiResult> => {
		return this.post(`${Constants.apiURL}/profile/removeLogin`, request);
	};

}
