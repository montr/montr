export interface IApiResult {
	success: boolean;
	errors?: IApiResultError[];
}

export interface IApiResultError {
	key: string;
	messages: string[];
}
