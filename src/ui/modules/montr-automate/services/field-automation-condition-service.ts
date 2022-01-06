import { Api } from "@montr-automate/module";
import { Guid, IDataField } from "@montr-core/models";
import { Fetcher } from "@montr-core/services";

export class FieldAutomationConditionService extends Fetcher {
	fields = async (metadataEntityTypeCode: string, metadataEntityUid: Guid | string): Promise<IDataField[]> => {
		return this.post(Api.fieldAutomationConditionFields, { metadataEntityTypeCode, metadataEntityUid });
	};
}
