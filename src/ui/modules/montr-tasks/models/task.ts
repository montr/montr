import { Guid, IIndexer } from "@montr-core/models";

export interface Task extends IIndexer {
    uid?: Guid;
    companyUid?: Guid;
    taskTypeUid?: Guid;
    code?: string;
    statusCode?: string;
    name?: string;
    url?: string;
    fields?: unknown;
}
