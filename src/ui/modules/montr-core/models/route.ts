import { RouteProps } from "react-router";

export interface IRoute extends RouteProps {
	layout?: string;
	layoutComponent?: React.ComponentType<any>;
}
