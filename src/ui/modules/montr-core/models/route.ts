import { ComponentType, LazyExoticComponent } from "react";
import { RouteProps } from "react-router";

export interface IRoute extends RouteProps {
	component: LazyExoticComponent<ComponentType<any>>;
	layout: string;
	layoutComponent?: React.ComponentType<any>;
}
