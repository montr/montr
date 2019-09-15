import { Constants } from "@montr-core/.";
import { Fetcher } from "@montr-core/services";
import { IApiResult, Guid } from "@montr-core/models";
import { IInvitation } from "../models";

interface IInsertInvitationRequest {
	eventUid: Guid;
	items: IInvitation[];
}

export class InvitationService extends Fetcher {
	insert = async (companyUid: Guid, request: IInsertInvitationRequest): Promise<IApiResult> => {
		return this.post(`${Constants.apiURL}/invitation/insert`, { companyUid, ...request });
	};

	update = async (companyUid: Guid, item: IInvitation): Promise<IApiResult> => {
		return this.post(`${Constants.apiURL}/invitation/update`, { companyUid, item });
	};

	delete = async (companyUid: Guid, uids: string[] | number[]): Promise<number> => {
		return this.post(`${Constants.apiURL}/invitation/delete`, { companyUid, uids });
	};
}
