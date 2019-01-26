export interface IMenu {
	id?: string;
	name?: string;
	icon?: string;
	url?: string;
	route?: string;
	items?: IMenu[];
}
