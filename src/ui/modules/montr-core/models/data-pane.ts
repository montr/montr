export interface DataPane<TEntity> {
	key?: string;
	name?: string;
	icon?: string;
	component?: string;
}

export interface PaneProps<TEntity> {
	data: TEntity;
	ref?: any;
	// [key: string]: any;
}
