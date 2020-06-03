import { Fetcher } from "./fetcher";
import { Guid, IAutomation, IApiResult } from "../models";
import { Constants } from "..";

interface IManageAutomationRequest {
	entityTypeCode: string;
	entityTypeUid: Guid | string;
	item: IAutomation;
}

interface IDeleteAutomationRequest {
	entityTypeCode: string;
	entityTypeUid: Guid | string;
	uids: string[] | number[];
}

export class AutomationService extends Fetcher {

	get = async (entityTypeCode: string, entityTypeUid: Guid | string, uid: Guid): Promise<IAutomation> => {
		return this.post(`${Constants.apiURL}/automation/get`, { entityTypeCode, entityTypeUid, uid });
	};

	insert = async (request: IManageAutomationRequest): Promise<IApiResult> => {
		return this.post(`${Constants.apiURL}/automation/insert`, request);
	};

	update = async (request: IManageAutomationRequest): Promise<IApiResult> => {
		return this.post(`${Constants.apiURL}/automation/update`, request);
	};

	delete = async (request: IDeleteAutomationRequest): Promise<IApiResult> => {
		return this.post(`${Constants.apiURL}/automation/delete`, request);
	};

}
