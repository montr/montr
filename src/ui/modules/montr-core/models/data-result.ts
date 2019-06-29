import { IApiResult } from "@montr-core/models";

export interface IDataResult<TResult> extends IApiResult {
	totalCount: number;
	rows: TResult[];
}
