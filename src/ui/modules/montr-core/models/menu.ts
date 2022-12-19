export interface IMenu {
	id?: string;
	name?: string | React.ReactNode;
	icon?: string;
	url?: string;
	route?: string | ((...args: any[]) => string);
	disabled?: boolean;
	onClick?: (...args: any[]) => void;
	items?: IMenu[];
}
