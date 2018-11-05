export interface DataColumn {
    key: string;
    path: string;
    name: string;
    align: DataColumnAlign,
    sortable: string;
    width?: number;
}

export enum DataColumnAlign {
    Left,
    Right,
    Center
}