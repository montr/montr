import { Fetcher } from "@montr-core/services";
import { Constants } from "@montr-core/constants";
import { IApiResult } from "@montr-core/models";
import { IProfileModel } from "../models";

export class ProfileService extends Fetcher {

	get = async (): Promise<IProfileModel> => {
		return this.post(`${Constants.apiURL}/profile/get`);
	};

	update = async (request: IProfileModel): Promise<IApiResult> => {
		return this.post(`${Constants.apiURL}/profile/update`, request);
	};

}
