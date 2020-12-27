import { Guid, IIndexer } from "@montr-core/models";

export interface IClassifier extends IIndexer {
	uid?: Guid | string;
	statusCode?: string;
	code?: string;
	name?: string;
	isActive?: boolean;
	isSystem?: boolean;
	url?: string;
}
