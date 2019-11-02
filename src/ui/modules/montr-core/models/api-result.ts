import { Guid } from ".";

export interface IApiResult {
	success: boolean;
	uid?: Guid;
	affectedRows?: number;
	errors?: IApiResultError[];
	redirectUrl?: string;
}

export interface IApiResultError {
	key: string;
	messages: string[];
}
