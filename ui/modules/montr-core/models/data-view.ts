import { IDataColumn, IPane } from ".";

export interface IDataView<TEntity> {
	id: string;
	columns?: IDataColumn[];
	panes?: IPane<TEntity>[]
}
