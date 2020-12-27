import { Guid } from "@montr-core/models";
import { IClassifier } from ".";

export interface Numerator extends IClassifier {
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
