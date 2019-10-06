import { IIndexer, Guid } from "@montr-core/models";

export interface IEvent extends IIndexer {
	uid?: Guid;
	id?: number;
	templateUid?: Guid;
	configCode?: string;
	statusCode?: string;
	name?: string;
	description?: string;
}
