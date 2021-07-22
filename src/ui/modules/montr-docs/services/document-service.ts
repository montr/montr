import { DataView, Guid } from "@montr-core/models";
import { Fetcher } from "@montr-core/services";
import { IDocument } from "../models";
import { Api } from "../module";

export class DocumentService extends Fetcher {

	metadata = async<TEntity>(documentUid: Guid, viewId: string): Promise<DataView<TEntity>> => {
		return await this.post(Api.documentMetadata, { documentUid, viewId });
	};

	get = async (uid: Guid | string): Promise<IDocument> => {
		return this.post(Api.documentGet, { uid });
	};
}
