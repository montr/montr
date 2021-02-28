import { Fetcher } from "@montr-core/services";
import { Constants } from "@montr-core/constants";
import { ApiResult } from "@montr-core/models";
import { ProfileModel, ChangePasswordModel, SetPasswordModel, UserLoginInfo } from "../models";

export class ProfileService extends Fetcher {

	get = async (): Promise<ProfileModel> => {
		return this.post(`${Constants.apiURL}/profile/get`);
	};

	update = async (request: ProfileModel): Promise<ApiResult> => {
		return this.post(`${Constants.apiURL}/profile/update`, request);
	};

	changeEmail = async (request: ProfileModel): Promise<ApiResult> => {
		return this.post(`${Constants.apiURL}/profile/changeEmail`, request);
	};

	changePhone = async (request: ProfileModel): Promise<ApiResult> => {
		return this.post(`${Constants.apiURL}/profile/changePhone`, request);
	};

	changePassword = async (request: ChangePasswordModel): Promise<ApiResult> => {
		return this.post(`${Constants.apiURL}/profile/changePassword`, request);
	};

	setPassword = async (request: SetPasswordModel): Promise<ApiResult> => {
		return this.post(`${Constants.apiURL}/profile/setPassword`, request);
	};

	externalLogins = async (): Promise<UserLoginInfo[]> => {
		return this.post(`${Constants.apiURL}/profile/externalLogins`, {});
	};

	linkLoginCallback = async (): Promise<ApiResult> => {
		return this.post(`${Constants.apiURL}/profile/linkLoginCallback`, {});
	};

	removeLogin = async (request: UserLoginInfo): Promise<ApiResult> => {
		return this.post(`${Constants.apiURL}/profile/removeLogin`, request);
	};

}
