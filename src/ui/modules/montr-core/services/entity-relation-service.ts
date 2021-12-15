import { EntityRelation, Guid } from "../models";
import { Api } from "../module";
import { Fetcher } from "./fetcher";

interface EntityRelationSearchRequest {
	entityTypeCode: string;
	entityUid: Guid | string;
}

export class EntityRelationService extends Fetcher {

	load = async (request: EntityRelationSearchRequest): Promise<EntityRelation[]> => {
		return this.post(Api.entityRelationList, request);
	};
}
