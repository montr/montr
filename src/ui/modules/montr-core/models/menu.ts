export interface IMenu {
	id?: string;
	name?: string;
	icon?: string;
	url?: string;
	route?: string;
	onClick?: (...args: any[]) => void;
	items?: IMenu[];
}
