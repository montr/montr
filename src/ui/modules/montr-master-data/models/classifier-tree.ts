import { Guid } from "@montr-core/models";
import { ClassifierGroup } from ".";

export interface ClassifierTree {
	uid?: Guid;
	code?: string;
	name?: string;
	children?: ClassifierGroup[];
}
