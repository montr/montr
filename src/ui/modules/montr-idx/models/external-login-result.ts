import { IExternalRegisterUser } from ".";
import { IApiResult } from "@montr-core/models";

export interface IExternalLoginResult extends IApiResult {
	register?: IExternalRegisterUser;
}
