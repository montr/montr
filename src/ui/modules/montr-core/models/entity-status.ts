import { Guid } from ".";

export interface EntityStatus {
	uid: Guid;
	displayOrder: number;
	code: string;
	name: string;
}
