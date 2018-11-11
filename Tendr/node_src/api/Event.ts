import Guid from "./Guid";

export interface Event {

	[key:string]: any; // Add index signature
	
	uid: Guid;
	id: number;
	configCode: string;
	statusCode: string;
	name: string;
	description: string;
}