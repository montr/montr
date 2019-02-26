export interface IDataColumn {
	key: string;
	path: string;
	name: string;
	align?: "left" | "right" | "center";
	sortable: boolean;
	defaultSortOrder?: string;
	width?: number;
	urlProperty?: string;
}
