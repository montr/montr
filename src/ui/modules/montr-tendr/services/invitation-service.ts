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

	get = async (uid: Guid | string): Promise<IInvitation> => {
		return this.post(`${Constants.apiURL}/invitation/get`, { uid });
	};

	insert = async (request: IInsertInvitationRequest): Promise<IApiResult> => {
		return this.post(`${Constants.apiURL}/invitation/insert`, request);
	};

	update = async (request: IUpdateInvitationRequest): Promise<IApiResult> => {
		return this.post(`${Constants.apiURL}/invitation/update`, request);
	};

	delete = async (uids: string[] | number[]): Promise<IApiResult> => {
		return this.post(`${Constants.apiURL}/invitation/delete`, { uids });
	};
}
