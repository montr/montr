import { RouteProps } from "react-router";

export interface IRoute extends RouteProps {
	layout?: "public" | "private";
	layoutComponent?: React.ComponentType<any>;
	// routes?: IRoute[];
}
