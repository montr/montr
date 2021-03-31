import { Constants } from "@montr-core/.";
import { Fetcher } from "@montr-core/services";
import { ApiResult, Guid } from "@montr-core/models";
import { Invitation } from "../models";

interface InsertInvitationRequest {
	eventUid: Guid;
	items: Invitation[];
}

interface UpdateInvitationRequest {
	eventUid: Guid;
	item: Invitation;
}

export class InvitationService extends Fetcher {

	get = async (uid: Guid | string): Promise<Invitation> => {
		return this.post(`${Constants.apiURL}/invitation/get`, { uid });
	};

	insert = async (request: InsertInvitationRequest): Promise<ApiResult> => {
		return this.post(`${Constants.apiURL}/invitation/insert`, request);
	};

	update = async (request: UpdateInvitationRequest): Promise<ApiResult> => {
		return this.post(`${Constants.apiURL}/invitation/update`, request);
	};

	delete = async (uids: string[] | number[]): Promise<ApiResult> => {
		return this.post(`${Constants.apiURL}/invitation/delete`, { uids });
	};
}
