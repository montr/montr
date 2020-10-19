import { Fetcher } from "@montr-core/services/fetcher";
import { Automation, AutomationRuleType } from "../models/automation";
import { Guid, ApiResult } from "@montr-core/models";
import { Constants } from "@montr-core/.";

interface IManageAutomationRequest {
	entityTypeCode: string;
	entityTypeUid: Guid | string;
	item: Automation;
}

interface IDeleteAutomationRequest {
	entityTypeCode: string;
	entityTypeUid: Guid | string;
	uids: string[] | number[];
}

export class AutomationService extends Fetcher {

	get = async (entityTypeCode: string, entityTypeUid: Guid | string, uid: Guid): Promise<Automation> => {
		return this.post(`${Constants.apiURL}/automation/get`, { entityTypeCode, entityTypeUid, uid });
	};

	insert = async (request: IManageAutomationRequest): Promise<ApiResult> => {
		return this.post(`${Constants.apiURL}/automation/insert`, request);
	};

	update = async (request: IManageAutomationRequest): Promise<ApiResult> => {
		return this.post(`${Constants.apiURL}/automation/update`, request);
	};

	delete = async (request: IDeleteAutomationRequest): Promise<ApiResult> => {
		return this.post(`${Constants.apiURL}/automation/delete`, request);
	};

	actionTypes = async (): Promise<AutomationRuleType[]> => {
		return this.post(`${Constants.apiURL}/automation/actionTypes`, {});
	};

	conditionTypes = async (): Promise<AutomationRuleType[]> => {
		return this.post(`${Constants.apiURL}/automation/conditionTypes`, {});
	};
}
