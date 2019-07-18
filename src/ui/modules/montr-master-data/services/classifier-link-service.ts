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

	list = async (companyUid: Guid, request: IClassifierLinkSearchRequest): Promise<IDataResult<IClassifierLink>> => {
		return this.post(`${Constants.baseURL}/classifierLink/list`, { companyUid, ...request });
	};

	insert = async (companyUid: Guid, typeCode: string, groupUid: Guid, itemUid: string | Guid): Promise<IApiResult> => {
		return this.post(`${Constants.baseURL}/classifierLink/insert`, { companyUid, typeCode, groupUid, itemUid });
	};

	delete = async (companyUid: Guid, typeCode: string, groupUid: Guid, itemUid: string | Guid): Promise<IApiResult> => {
		return this.post(`${Constants.baseURL}/classifierLink/delete`, { companyUid, typeCode, groupUid, itemUid });
	};
}
