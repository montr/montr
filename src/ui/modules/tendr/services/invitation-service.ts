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
}
