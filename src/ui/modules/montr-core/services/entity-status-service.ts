import { Guid, IApiResult, EntityStatus } from "../models";
import { Constants } from "..";
import { Fetcher } from "./fetcher";

interface InsertEntityStatusRequest {
    entityTypeCode: string;
    entityUid: Guid | string;
    item: EntityStatus;
}

interface DeleteEntityStatusRequest {
    entityTypeCode: string;
    entityUid: Guid;
    codes: string[] | number[];
}

export class EntityStatusService extends Fetcher {

    insert = async (request: InsertEntityStatusRequest): Promise<IApiResult> => {
        return this.post(`${Constants.apiURL}/entityStatus/insert`, request);
    };

    delete = async (request: DeleteEntityStatusRequest): Promise<IApiResult> => {
        return this.post(`${Constants.apiURL}/entityStatus/delete`, request);
    };
}