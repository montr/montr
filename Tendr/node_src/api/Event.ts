import { Indexer, Guid } from ".";

export interface Event extends Indexer {
	uid: Guid;
	id: number;
	configCode: string;
	statusCode: string;
	name: string;
	description: string;
}