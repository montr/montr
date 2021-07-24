export interface DataColumn {
	key: string;
	type: string;
	path: string;
	name: string;
	align?: "left" | "right" | "center";
	sortable: boolean;
	defaultSortOrder?: SortOrder;
	width?: number;
	urlProperty?: string;
}

export declare type SortOrder = "Ascending" | "Descending";

export interface Paging {
	pageNo?: number;
	pageSize?: number;
	sortColumn?: string;
	sortOrder?: SortOrder;
	skipPaging?: boolean;
}
