import { ApiResult, Guid } from "@montr-core/models";
import { Fetcher } from "@montr-core/services";
import { Invitation } from "../models";
import { Api } from "../module";

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
		return this.post(Api.eventInvitationGet, { uid });
	};

	insert = async (request: InsertInvitationRequest): Promise<ApiResult> => {
		return this.post(Api.eventInvitationInsert, request);
	};

	update = async (request: UpdateInvitationRequest): Promise<ApiResult> => {
		return this.post(Api.eventInvitationUpdate, request);
	};

	delete = async (uids: string[] | number[]): Promise<ApiResult> => {
		return this.post(Api.eventInvitationDelete, { uids });
	};
}
