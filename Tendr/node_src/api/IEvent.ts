import { IIndexer, Guid } from ".";

export interface IEvent extends IIndexer {
	uid: Guid;
	id: number;
	configCode: string;
	statusCode: string;
	name: string;
	description: string;
}