import { ApiResult } from "@montr-core/models";

export interface DataResult<TResult> extends ApiResult {
	totalCount: number;
	rows: TResult[];
}
