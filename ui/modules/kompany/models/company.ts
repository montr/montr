import { Guid } from "@montr-core/.";

export interface ICompany {
	uid?: Guid;
	configCode?: string;
	statusCode?: string;
	name?: string;
}
