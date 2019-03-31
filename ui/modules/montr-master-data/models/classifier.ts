import { Guid, IIndexer } from "@montr-core/models";

export interface IClassifier extends IIndexer {
	uid?: Guid;
	statusCode?: string;
	code?: string;
	name?: string;
	url?: string;
}
