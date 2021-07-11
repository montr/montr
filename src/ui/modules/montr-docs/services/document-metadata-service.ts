import { Fetcher } from "@montr-core/services";
import { DataView, Guid } from "@montr-core/models";
import { Api } from "../module";

export class DocumentMetadataService extends Fetcher {

	view = async<TEntity>(documentTypeUid: Guid, viewId: string): Promise<DataView<TEntity>> => {

		const data: DataView<TEntity> =
			await this.post(Api.documentMetadataView, { documentTypeUid, viewId });

		return data;
	};

}
