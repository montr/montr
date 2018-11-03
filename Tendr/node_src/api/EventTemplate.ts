import Guid from "./Guid";

export interface EventTemplate {
    id: Guid;
    eventType: number;
    name: string;
    description: string;
}