import { Guid, IIndexer } from "@montr-core/models";

export interface IDocument extends IIndexer {
	uid?: Guid | string;
	configCode?: string;
	statusCode?: string;
	documentNumber?: string;
	documentDate?: Date;
	name?: string;
	url?: string;
}
