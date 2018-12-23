import { IIndexer } from "@montr-core/.";

export interface ICompany extends IIndexer {
	id?: number;
	configCode?: string;
	statusCode?: string;
	name?: string;
}