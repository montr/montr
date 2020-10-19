import { DataColumn, Pane, IDataField } from ".";

export interface IDataView<TEntity> {
	id: string;
	columns?: DataColumn[];
	fields?: IDataField[];
	panes?: Pane<TEntity>[];
}
