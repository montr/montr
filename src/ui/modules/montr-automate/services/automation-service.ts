import { ApiResult, Guid, IDataField } from "@montr-core/models";
import { Fetcher } from "@montr-core/services/fetcher";
import { Automation, AutomationRuleType } from "../models/automation";
import { Api } from "../module";

interface IManageAutomationRequest {
	entityTypeCode: string;
	entityUid: Guid | string;
	item: Automation;
}

interface IDeleteAutomationRequest {
	entityTypeCode: string;
	entityUid: Guid | string;
	uids: string[] | number[];
}

export class AutomationService extends Fetcher {

	metadata = async (actionTypeCode: string, conditionTypeCode: string): Promise<IDataField[]> => {
		return this.post(Api.automationMetadata, { actionTypeCode, conditionTypeCode });
	};

	get = async (entityTypeCode: string, entityUid: Guid | string, uid: Guid): Promise<Automation> => {
		return this.post(Api.automationGet, { entityTypeCode, entityUid, uid });
	};

	insert = async (request: IManageAutomationRequest): Promise<ApiResult> => {
		return this.post(Api.automationInsert, request);
	};

	update = async (request: IManageAutomationRequest): Promise<ApiResult> => {
		return this.post(Api.automationUpdate, request);
	};

	delete = async (request: IDeleteAutomationRequest): Promise<ApiResult> => {
		return this.post(Api.automationDelete, request);
	};

	actionTypes = async (): Promise<AutomationRuleType[]> => {
		return this.post(Api.automationActionTypes, {});
	};

	conditionTypes = async (): Promise<AutomationRuleType[]> => {
		return this.post(Api.automationConditionTypes, {});
	};
}
