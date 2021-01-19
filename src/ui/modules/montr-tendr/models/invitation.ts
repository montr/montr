import { IIndexer, Guid } from "@montr-core/models";

export interface Invitation extends IIndexer {
	uid?: Guid;
	eventUid?: Guid,
	statusCode?: string;
	counterpartyUid?: Guid;
}

export interface InvitationListItem extends IIndexer {
	uid?: Guid;
	eventUid?: Guid,
	statusCode?: string;
	counterpartyUid?: Guid;
	counterpartyName?: string;
	email?: string;
}
