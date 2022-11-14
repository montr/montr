import { ApiResult } from "@montr-core/models/api-result";
import { DataView } from "@montr-core/models/data-view";
import { Guid } from "@montr-core/models/guid";
import { Fetcher } from "@montr-core/services";
import { Api } from "../module";

export class SettingsService extends Fetcher {
	metadata = async<TEntity>(entityTypeCode: string, entityUid: Guid, settingsTypeCode: string): Promise<DataView<TEntity>> => {
		return await this.post(Api.settingsMetadata, { entityTypeCode, entityUid, settingsTypeCode });
	};

	get = async (entityTypeCode: string, entityUid: Guid, settingsTypeCode: string): Promise<ApiResult<unknown>> => {
		return await this.post(Api.settingsGet, { entityTypeCode, entityUid, settingsTypeCode });
	};

	update = async (entityTypeCode: string, entityUid: Guid, settingsTypeCode: string, values: any): Promise<ApiResult> => {
		return await this.post(Api.settingsUpdate, {
			entityTypeCode, entityUid, settingsTypeCode,
			values: { __typeCode: settingsTypeCode, ...values }
		});
	};
}
