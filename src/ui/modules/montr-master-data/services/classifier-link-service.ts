import { ApiResult, DataResult, Guid } from "@montr-core/models";
import { Fetcher } from "@montr-core/services";
import { ClassifierLink } from "../models";
import { Api } from "../module";

interface IClassifierLinkSearchRequest {
	typeCode: string;
	groupUid?: Guid;
	itemUid?: Guid | string;
}

export class ClassifierLinkService extends Fetcher {

	list = async (request: IClassifierLinkSearchRequest): Promise<DataResult<ClassifierLink>> => {
		return this.post(Api.classifierLinkList, request);
	};

	insert = async (typeCode: string, groupUid: Guid, itemUid: string | Guid): Promise<ApiResult> => {
		return this.post(Api.classifierLinkInsert, { typeCode, groupUid, itemUid });
	};

	delete = async (typeCode: string, groupUid: Guid, itemUid: string | Guid): Promise<ApiResult> => {
		return this.post(Api.classifierLinkDelete, { typeCode, groupUid, itemUid });
	};
}
