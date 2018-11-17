import { IIndexer, Guid } from ".";

export interface IEvent extends IIndexer {
	id?: number;
	configCode?: string;
	statusCode?: string;
	name?: string;
	description?: string;
}