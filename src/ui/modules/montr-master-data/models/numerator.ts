import { Guid, IIndexer } from "@montr-core/models";

export interface INumerator extends IIndexer {
	uid?: Guid | string;
	entityTypeCode?: string;
	pattern?: string;
	key_tags?: string[];
	name?: string;
	periodicity?: "None" | "Day" | "Month" | "Quarter" | "Year";
	isActive?: boolean;
	isSystem?: boolean;
	url?: string;
}
