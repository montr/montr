import { IDataColumn, IPane, IDataField } from ".";

export interface IDataView<TEntity> {
	id: string;
	columns?: IDataColumn[];
	fields?: IDataField[];
	panes?: IPane<TEntity>[];
}
