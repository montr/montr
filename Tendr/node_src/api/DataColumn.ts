export interface DataColumn {
    key: string;
    path: string;
    name: string;
    align?: "left" | "right" | "center",
    sortable: boolean;
    width?: number;
    urlProperty?: string;
}

export enum DataColumnAlign {
    Left,
    Right,
    Center
}