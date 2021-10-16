import { ApiResult } from "@montr-core/models";
import { Fetcher } from "@montr-core/services";
import { ChangePasswordModel, ProfileModel, SetPasswordModel, UserLoginInfo } from "../models";
import { Api } from "../module";

export class ProfileService extends Fetcher {

	get = async (): Promise<ProfileModel> => {
		return this.post(Api.profileGet);
	};

	update = async (request: ProfileModel): Promise<ApiResult> => {
		return this.post(Api.profileUpdate, request);
	};

	changeEmail = async (request: ProfileModel): Promise<ApiResult> => {
		return this.post(Api.profileChangeEmail, request);
	};

	changePhone = async (request: ProfileModel): Promise<ApiResult> => {
		return this.post(Api.profileChangePhone, request);
	};

	changePassword = async (request: ChangePasswordModel): Promise<ApiResult> => {
		return this.post(Api.profileChangePassword, request);
	};

	setPassword = async (request: SetPasswordModel): Promise<ApiResult> => {
		return this.post(Api.profileSetPassword, request);
	};

	externalLogins = async (): Promise<UserLoginInfo[]> => {
		return this.post(Api.profileExternalLogins, {});
	};

	linkLoginCallback = async (): Promise<ApiResult> => {
		return this.post(Api.profileLinkLoginCallback, {});
	};

	removeLogin = async (request: UserLoginInfo): Promise<ApiResult> => {
		return this.post(Api.profileRemoveLogin, request);
	};

}
