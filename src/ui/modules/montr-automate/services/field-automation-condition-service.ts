import { Fetcher } from "@montr-core/services";
import { Guid, IDataField } from "@montr-core/models";
import { Constants } from "@montr-core/.";

export class FieldAutomationConditionService extends Fetcher {
	fields = async (entityTypeCode: string, entityTypeUid: Guid | string): Promise<IDataField[]> => {
		return this.post(`${Constants.apiURL}/fieldAutomationCondition/fields`, { entityTypeCode, entityTypeUid });
	};
}
