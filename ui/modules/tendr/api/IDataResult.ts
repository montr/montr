import { IApiResult } from ".";

export interface IDataResult<TResult> extends IApiResult {
	totalCount: number;
	rows: TResult[];
}