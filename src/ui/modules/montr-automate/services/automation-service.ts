import { ApiResult, Guid, IDataField } from "@montr-core/models";
import { Fetcher } from "@montr-core/services/fetcher";
import { AutomationAction, AutomationCondition, AutomationRuleType } from "../models/automation";
import { Api } from "../module";

interface IUpdateAutomationRulesRequest {
	automationUid: Guid;
	conditions?: AutomationCondition[];
	actions?: AutomationAction[];
}

export class AutomationService extends Fetcher {

	actionMetadata = async (entityTypeCode: string, action: AutomationAction): Promise<IDataField[]> => {
		return this.post(Api.automationActionMetadata, { entityTypeCode, action });
	};

	conditionMetadata = async (entityTypeCode: string, condition: AutomationCondition): Promise<IDataField[]> => {
		return this.post(Api.automationConditionMetadata, { entityTypeCode, condition });
	};

	actionTypes = async (): Promise<AutomationRuleType[]> => {
		return this.post(Api.automationActionTypes, {});
	};

	conditionTypes = async (): Promise<AutomationRuleType[]> => {
		return this.post(Api.automationConditionTypes, {});
	};

	updateRules = async (request: IUpdateAutomationRulesRequest): Promise<ApiResult> => {
		return this.post(Api.automationUpdateRules, request);
	};
}
