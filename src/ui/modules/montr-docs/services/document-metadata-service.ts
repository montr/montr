import { Fetcher } from "@montr-core/services";
import { IDataView, Guid } from "@montr-core/models";
import { Constants } from "@montr-core/.";

export class DocumentMetadataService extends Fetcher {

	load = async<TEntity>(documentTypeUid: Guid): Promise<IDataView<TEntity>> => {

		const data: IDataView<TEntity> =
			await this.post(`${Constants.apiURL}/DocumentMetadata/View`, { documentTypeUid });

		return data;
	};

}
