import { Guid } from "@montr-core/models";

export interface ICompany {
	uid?: Guid;
	configCode?: string;
	statusCode?: string;
	name?: string;
}
