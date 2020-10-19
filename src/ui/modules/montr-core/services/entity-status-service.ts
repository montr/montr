import { Guid, ApiResult, EntityStatus } from "../models";
import { Fetcher } from "./fetcher";
import { Api } from "../module";

interface GetEntityStatusRequest {
    entityTypeCode: string;
    entityUid: Guid | string;
    uid: Guid;
}

interface ManageEntityStatusRequest {
    entityTypeCode: string;
    entityUid: Guid | string;
    item: EntityStatus;
}

interface DeleteEntityStatusRequest {
    entityTypeCode: string;
    entityUid: Guid;
    uids: string[] | number[];
}

export class EntityStatusService extends Fetcher {

    get = async (request: GetEntityStatusRequest): Promise<EntityStatus> => {
        return this.post(Api.entityStatusGet, request);
    };

    insert = async (request: ManageEntityStatusRequest): Promise<ApiResult> => {
        return this.post(Api.entityStatusInsert, request);
    };

    update = async (request: ManageEntityStatusRequest): Promise<ApiResult> => {
        return this.post(Api.entityStatusUpdate, request);
    };

    delete = async (request: DeleteEntityStatusRequest): Promise<ApiResult> => {
        return this.post(Api.entityStatusDelete, request);
    };
}