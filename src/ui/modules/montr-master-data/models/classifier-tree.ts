import { Guid } from "@montr-core/models";
import { IClassifierGroup } from ".";

export interface IClassifierTree {
	uid?: Guid;
	code?: string;
	name?: string;
	children?: IClassifierGroup[];
}
