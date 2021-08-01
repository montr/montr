import { ApiResult } from "./";

export interface DataResult<TResult> extends ApiResult {
	totalCount: number;
	rows: TResult[];
}
