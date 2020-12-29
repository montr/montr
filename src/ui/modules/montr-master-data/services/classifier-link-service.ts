import { Fetcher } from "@montr-core/services";
import { Constants } from "@montr-core/.";
import { Guid, ApiResult, DataResult } from "@montr-core/models";
import { ClassifierLink } from "../models";

interface IClassifierLinkSearchRequest {
	typeCode: string;
	groupUid?: Guid;
	itemUid?: Guid | string;
}

export class ClassifierLinkService extends Fetcher {

	list = async (request: IClassifierLinkSearchRequest): Promise<DataResult<ClassifierLink>> => {
		return this.post(`${Constants.apiURL}/classifierLink/list`, request);
	};

	insert = async (typeCode: string, groupUid: Guid, itemUid: string | Guid): Promise<ApiResult> => {
		return this.post(`${Constants.apiURL}/classifierLink/insert`, { typeCode, groupUid, itemUid });
	};

	delete = async (typeCode: string, groupUid: Guid, itemUid: string | Guid): Promise<ApiResult> => {
		return this.post(`${Constants.apiURL}/classifierLink/delete`, { typeCode, groupUid, itemUid });
	};
}
