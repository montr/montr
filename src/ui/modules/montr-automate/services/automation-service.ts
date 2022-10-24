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

	metadata = async (entityTypeCode: string, actionTypeCode: string, conditionTypeCode: string): Promise<IDataField[]> => {
		return this.post(Api.automationMetadata, { entityTypeCode, actionTypeCode, conditionTypeCode });
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
