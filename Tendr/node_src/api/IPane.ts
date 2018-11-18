export interface IPane<TEntity> {
	key?: string;
	name?: string;
	icon?: string;
	component?: React.ComponentClass<IPaneProps<TEntity>>;
}

export interface IPaneProps<TEntity> {
	data: TEntity;
	ref?: any;
	// [key: string]: any;
}