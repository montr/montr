import { IIndexer, Guid } from "@montr-core/models";

export interface IInvitation extends IIndexer {
	uid?: Guid;
	eventUid?: Guid,
	statusCode?: string;
	counterpartyUid?: Guid,
}
