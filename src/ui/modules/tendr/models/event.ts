import { IIndexer, Guid } from "@montr-core/models";

export interface IEvent extends IIndexer {
	uid?: Guid;
	id?: number;
	configCode?: string;
	statusCode?: string;
	companyUid?: Guid,
	name?: string;
	description?: string;
}
