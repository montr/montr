import { DataView } from "@montr-core/models";
import { Fetcher } from "@montr-core/services";
import { Api } from "../module";

export class EventMetadataService extends Fetcher {

	load = async<TEntity>(viewId: string): Promise<DataView<TEntity>> => {

		const data: DataView<TEntity> =
			await this.post(Api.eventMetadataView, { viewId });

		return data;
	};

}
