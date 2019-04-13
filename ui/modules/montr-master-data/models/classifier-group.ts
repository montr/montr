import { Guid } from "@montr-core/models";

export interface IClassifierGroup {
	uid?: Guid;
	code?: string;
	name?: string;
	parentUid?: Guid;
	children?: IClassifierGroup[];
}
