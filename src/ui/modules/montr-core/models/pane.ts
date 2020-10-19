export interface Pane<TEntity> {
	key?: string;
	name?: string;
	icon?: string;
	component?: React.ComponentClass<PaneProps<TEntity>>;
}

export interface PaneProps<TEntity> {
	data: TEntity;
	ref?: any;
	// [key: string]: any;
}