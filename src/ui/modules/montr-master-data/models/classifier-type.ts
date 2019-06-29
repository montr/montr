import { Guid } from "@montr-core/models";

export interface IClassifierType {
	uid?: Guid;
	code?: string;
	name?: string;
	description?: string;
	isSystem?: boolean;
	hierarchyType?: "None" | "Groups" | "Items";
}
