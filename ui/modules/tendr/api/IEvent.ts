import { IIndexer } from "@montr-core/models";

export interface IEvent extends IIndexer {
	id?: number;
	configCode?: string;
	statusCode?: string;
	name?: string;
	description?: string;
}
