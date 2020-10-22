import { DataColumn, DataPane, IDataField } from ".";

export interface DataView<TEntity> {
	id: string;
	columns?: DataColumn[];
	fields?: IDataField[];
	panes?: DataPane<TEntity>[];
}
