import { Guid } from ".";

export interface ApiResult<TData = any> {
	success: boolean;
	message?: string;
	uid?: Guid;
	affectedRows?: number;
	concurrencyStamp?: string;
	errors?: ApiResultError[];
	redirectUrl?: string;
	data?: TData;
}

export interface ApiResultError {
	key: string;
	messages: string[];
}

export interface ProblemDetails {
	detail?: string;
	instance?: string;
	status?: number;
	title?: string;
	type?: string;
}

export interface ValidationProblemDetails extends ProblemDetails {
	errors?: Map<string, string[]>;
}
