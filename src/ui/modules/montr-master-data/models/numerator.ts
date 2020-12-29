import { Guid } from "@montr-core/models";
import { Classifier } from ".";

export interface Numerator extends Classifier {
	entityTypeCode?: string;
	pattern?: string;
	key_tags?: string[];
	periodicity?: "None" | "Day" | "Month" | "Quarter" | "Year";
}

export interface NumeratorEntity {
	isAutoNumbering: string;
	entityTypeCode: string;
	entityUid: Guid;
	numeratorUid?: Guid;
}
