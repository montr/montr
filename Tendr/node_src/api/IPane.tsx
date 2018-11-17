export interface IPane<TEntity> {
	key?: string;
	title?: string;
	icon?: string;
	component?: React.ComponentClass<IPaneProps<TEntity>>;
}

export interface IPaneProps<TEntity> {
	data: TEntity;
}