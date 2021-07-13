import { DataView, Guid } from "@montr-core/models";
import { Fetcher } from "@montr-core/services";
import { Api } from "../module";

export class DocumentMetadataService extends Fetcher {

	view = async<TEntity>(documentUid: Guid, viewId: string): Promise<DataView<TEntity>> => {

		const data: DataView<TEntity> =
			await this.post(Api.documentMetadataView, { documentUid, viewId });

		return data;
	};

}
