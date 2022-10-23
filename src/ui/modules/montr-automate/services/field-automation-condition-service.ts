import { Api } from "@montr-automate/module";
import { IDataField } from "@montr-core/models";
import { Fetcher } from "@montr-core/services";

export class FieldAutomationConditionService extends Fetcher {
	fields = async (entityTypeCode: string/* , entityUid: Guid | string */): Promise<IDataField[]> => {
		return this.post(Api.fieldAutomationConditionFields, { entityTypeCode/* , entityUid */ });
	};
}
