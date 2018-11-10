import Guid from "./Guid";

export interface Event {
    uid: Guid;
    id: number;
    configCode: string;
    statusCode: string;
    name: string;
    description: string;
}