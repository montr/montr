import { Guid } from "@montr-core/models";

export interface User {
    uid: Guid;
    userName: string;
    lastName: string;
    firstName: string;
    email: string;
    phoneNumber: string;
    concurrencyStamp: string;
}
