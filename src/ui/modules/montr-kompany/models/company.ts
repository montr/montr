import { Guid } from "@montr-core/models";

export interface Company {
	uid?: Guid;
	configCode?: string;
	statusCode?: string;
	name?: string;
}
