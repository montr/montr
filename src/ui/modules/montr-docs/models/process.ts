import { Guid, IIndexer } from "@montr-core/models";

export interface Process extends IIndexer {
    uid?: Guid | string;
}
