import { ApiResult } from "@montr-core/models/api-result";
import { DataView } from "@montr-core/models/data-view";
import { Guid } from "@montr-core/models/guid";
import { Fetcher } from "@montr-core/services";
import { Api } from "../module";

export class SettingsService extends Fetcher {
	metadata = async<TEntity>(entityTypeCode: string, entityUid: Guid): Promise<DataView<TEntity>> => {
		return await this.post(Api.settingsMetadata, { entityTypeCode, entityUid });
	};

	update = async (entityTypeCode: string, entityUid: Guid, typeCode: string, values: any): Promise<ApiResult> => {
		return await this.post(Api.settingsUpdate, { entityTypeCode, entityUid, values: { typeCode, ...values } });
	};
}
