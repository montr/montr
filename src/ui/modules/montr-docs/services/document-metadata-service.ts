import { Fetcher } from "@montr-core/services";
import { DataView, Guid } from "@montr-core/models";
import { Constants } from "@montr-core/.";

export class DocumentMetadataService extends Fetcher {

	load = async<TEntity>(documentTypeUid: Guid): Promise<DataView<TEntity>> => {

		const data: DataView<TEntity> =
			await this.post(`${Constants.apiURL}/DocumentMetadata/View`, { documentTypeUid });

		return data;
	};

}
