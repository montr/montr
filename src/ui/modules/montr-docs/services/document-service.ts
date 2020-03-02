import { Guid } from "@montr-core/models";
import { Fetcher } from "@montr-core/services";
import { Api } from "../module";
import { IDocument } from "../models";

export class DocumentService extends Fetcher {
	get = async (uid: Guid | string): Promise<IDocument> => {
		return this.post(Api.documentGet, { uid });
	};
}
