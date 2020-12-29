import { Guid } from "@montr-core/models";

export interface ClassifierType {
	uid?: Guid;
	code?: string;
	name?: string;
	description?: string;
	isSystem?: boolean;
	hierarchyType?: "None" | "Groups" | "Items";
}
