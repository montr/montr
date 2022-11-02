import { Guid } from ".";

export interface DataPane<TEntity> {
	key: string;
	name: string;
	description: string;
	icon: string;
	component: string;
	props: DataPaneProps<TEntity>;
}

export interface DataPaneProps<TEntity> {
	entityTypeCode: string;
	entityUid: Guid;
	data?: TEntity;
	ref?: unknown; // any
	[key: string]: unknown;
}
