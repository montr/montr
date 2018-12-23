import { IApiResult } from "@montr-core/api/";

export interface IDataResult<TResult> extends IApiResult {
	totalCount: number;
	rows: TResult[];
}
