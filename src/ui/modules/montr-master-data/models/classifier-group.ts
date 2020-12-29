import { Guid } from "@montr-core/models";

export interface ClassifierGroup {
	uid?: Guid;
	code?: string;
	name?: string;
	parentUid?: Guid;
	treeUid?: Guid;
	children?: ClassifierGroup[];
}
