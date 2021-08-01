import { Button, DataColumn, DataPane, IDataField } from ".";

export interface DataView<TEntity> {
	id: string;
	toolbar?: Button[];
	panes?: DataPane<TEntity>[];
	columns?: DataColumn[];
	fields?: IDataField[];
}
