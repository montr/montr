export interface IClassifierType {
	code?: string;
	name?: string;
	isSystem?: boolean;
	hierarchyType: "None" | "Groups" | "Items";
}
