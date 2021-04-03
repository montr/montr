import { Classifier } from "@montr-master-data/models";

export interface Role extends Classifier {
    concurrencyStamp: string;
}
