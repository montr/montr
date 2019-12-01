import { Guid } from ".";

export interface IApiResult<TData = any> {
	success: boolean;
	message?: string;
	uid?: Guid;
	affectedRows?: number;
	errors?: IApiResultError[];
	redirectUrl?: string;
	data?: TData;
}

export interface IApiResultError {
	key: string;
	messages: string[];
}

export interface IProblemDetails {
	detail?: string;
	instance?: string;
	status?: number;
	title?: string;
	type?: string;
}

export interface IValidationProblemDetails extends IProblemDetails {
	errors?: Map<string, string[]>;
}
