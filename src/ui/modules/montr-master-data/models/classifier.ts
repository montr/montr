import { Guid, IIndexer } from "@montr-core/models";

export interface Classifier extends IIndexer {
	uid?: Guid;
	statusCode?: string;
	code?: string;
	name?: string;
	isActive?: boolean;
	isSystem?: boolean;
	url?: string;
}
