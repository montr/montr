import { IIndexer } from ".";

export interface ICompany extends IIndexer {
	id?: number;
	configCode?: string;
	statusCode?: string;
	name?: string;
}