import { Guid } from "@montr-core/models";

export interface Role {
    uid: Guid;
    name: string;
    concurrencyStamp: string;
}
