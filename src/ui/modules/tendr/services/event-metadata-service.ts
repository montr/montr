import { Fetcher } from "@montr-core/services";
import { IDataView } from "@montr-core/models";
import { Constants } from "@montr-core/.";

export class EventMetadataService extends Fetcher {

	load = async<TEntity>(viewId: string): Promise<IDataView<TEntity>> => {

		const data: IDataView<TEntity> =
			await this.post(`${Constants.apiURL}/EventMetadata/View`, { viewId });

		return data;
	}

}
