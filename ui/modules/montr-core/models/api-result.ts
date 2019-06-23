export interface IApiResult {
	success: boolean;
	affectedRows?: number;
	errors?: IApiResultError[];
}

export interface IApiResultError {
	key: string;
	messages: string[];
}
