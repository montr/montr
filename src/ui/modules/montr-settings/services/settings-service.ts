import { DataView } from "@montr-core/models/data-view";
import { Guid } from "@montr-core/models/guid";
import { Fetcher } from "@montr-core/services";
import { Api } from "../module";

export class SettingsService extends Fetcher {
	metadata = async<TEntity>(entityTypeCode: string, entityUid?: Guid): Promise<DataView<TEntity>> => {
		return await this.post(Api.settingsMetadata, { entityTypeCode, entityUid });
	};
}
