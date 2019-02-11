import { IDataColumn, IPane, IFormField } from ".";

export interface IDataView<TEntity> {
	id: string;
	columns?: IDataColumn[];
	fields?: IFormField[];
	panes?: IPane<TEntity>[];
}
