export interface IMenu {
	id?: string;
	name?: string;
	icon?: string;
	url?: string;
	route?: string | ((...args: any[]) => string);
	onClick?: (...args: any[]) => void;
	items?: IMenu[];
}
