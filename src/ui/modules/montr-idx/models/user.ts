import { Classifier } from "@montr-master-data/models";

export interface User extends Classifier {
    userName: string;
    lastName: string;
    firstName: string;
    email: string;
    phoneNumber: string;
    concurrencyStamp: string;
}
