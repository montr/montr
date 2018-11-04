import Guid from "./Guid";

export interface Event {
    id: Guid;
    eventType: number;
    number: string;
    name: string;
    description: string;
}