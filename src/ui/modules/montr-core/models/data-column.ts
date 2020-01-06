export interface IDataColumn {
	key: string;
	type: string;
	path: string;
	name: string;
	align?: "left" | "right" | "center";
	sortable: boolean;
	defaultSortOrder?: ISortOrder;
	width?: number;
	urlProperty?: string;
}

export declare type ISortOrder = "ascending" | "descending";

export interface IPaging {
	pageNo?: number;
	pageSize?: number;
	sortColumn?: string;
	sortOrder?: ISortOrder;
}
