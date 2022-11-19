import { ApiResult } from "@montr-core/models/api-result";
import { Guid } from "@montr-core/models/guid";
import { Fetcher } from "@montr-core/services";
import { SettingsBlock } from "@montr-settings/models/settings-block";
import { Api } from "../module";

export class SettingsService extends Fetcher {
	metadata = async (entityTypeCode: string, entityUid: string | Guid, category: string): Promise<SettingsBlock[]> => {
		return await this.post(Api.settingsMetadata, { entityTypeCode, entityUid, category });
	};

	get = async (entityTypeCode: string, entityUid: string | Guid, settingsTypeCode: string): Promise<ApiResult<unknown>> => {
		return await this.post(Api.settingsGet, { entityTypeCode, entityUid, settingsTypeCode });
	};

	update = async (entityTypeCode: string, entityUid: string | Guid, settingsTypeCode: string, values: any): Promise<ApiResult> => {
		return await this.post(Api.settingsUpdate, {
			entityTypeCode, entityUid, settingsTypeCode, values: { __typeCode: settingsTypeCode, ...values }
		});
	};
}
