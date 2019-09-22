import { Fetcher } from "@montr-core/services";
import { Constants } from "@montr-core/.";
import { Guid, IApiResult, IDataResult } from "@montr-core/models";
import { IClassifierLink } from "../models";

interface IClassifierLinkSearchRequest {
	typeCode: string;
	groupUid?: Guid;
	itemUid?: Guid | string;
}

export class ClassifierLinkService extends Fetcher {

	list = async (request: IClassifierLinkSearchRequest): Promise<IDataResult<IClassifierLink>> => {
		return this.post(`${Constants.apiURL}/classifierLink/list`, request);
	};

	insert = async (typeCode: string, groupUid: Guid, itemUid: string | Guid): Promise<IApiResult> => {
		return this.post(`${Constants.apiURL}/classifierLink/insert`, { typeCode, groupUid, itemUid });
	};

	delete = async (typeCode: string, groupUid: Guid, itemUid: string | Guid): Promise<IApiResult> => {
		return this.post(`${Constants.apiURL}/classifierLink/delete`, { typeCode, groupUid, itemUid });
	};
}
