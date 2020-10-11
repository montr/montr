import { Guid, IApiResult, IEntityStatus } from "../models";
import { Constants } from "..";
import { Fetcher } from "./fetcher";

interface IInsertEntityStatusRequest {
    entityTypeCode: string;
    entityUid: Guid | string;
    item: IEntityStatus;
}

export class EntityStatusService extends Fetcher {

    insert = async (request: IInsertEntityStatusRequest): Promise<IApiResult> => {
        return this.post(`${Constants.apiURL}/entityStatus/insert`, request);
    };

}