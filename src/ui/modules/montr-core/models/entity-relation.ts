import { Guid } from ".";

export interface EntityRelation {
	entityTypeCode: string;
	entityUid: Guid;
	relatedEntityTypeCode: string;
	relatedEntityUid: Guid;
	relationType: string;
}
