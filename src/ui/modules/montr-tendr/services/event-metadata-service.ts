import { Fetcher } from "@montr-core/services";
import { DataView } from "@montr-core/models";
import { Constants } from "@montr-core/.";

export class EventMetadataService extends Fetcher {

	load = async<TEntity>(viewId: string): Promise<DataView<TEntity>> => {

		const data: DataView<TEntity> =
			await this.post(`${Constants.apiURL}/EventMetadata/View`, { viewId });

		return data;
	};

}
