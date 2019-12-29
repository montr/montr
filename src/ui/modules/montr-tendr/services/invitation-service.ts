import { Constants } from "@montr-core/.";
import { Fetcher } from "@montr-core/services";
import { IApiResult, Guid } from "@montr-core/models";
import { IInvitation } from "../models";

interface IInsertInvitationRequest {
	eventUid: Guid;
	items: IInvitation[];
}

interface IUpdateInvitationRequest {
	eventUid: Guid;
	item: IInvitation;
}

export class InvitationService extends Fetcher {

	get = async (companyUid: Guid, uid: Guid | string): Promise<IInvitation> => {
		return this.post(`${Constants.apiURL}/invitation/get`, { companyUid, uid });
	};

	insert = async (companyUid: Guid, request: IInsertInvitationRequest): Promise<IApiResult> => {
		return this.post(`${Constants.apiURL}/invitation/insert`, { companyUid, ...request });
	};

	update = async (companyUid: Guid, request: IUpdateInvitationRequest): Promise<IApiResult> => {
		return this.post(`${Constants.apiURL}/invitation/update`, { companyUid, ...request });
	};

	delete = async (companyUid: Guid, uids: string[] | number[]): Promise<IApiResult> => {
		return this.post(`${Constants.apiURL}/invitation/delete`, { companyUid, uids });
	};
}
