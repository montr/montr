import React from "react";
import { Route, RouteComponentProps, RouteProps } from "react-router";

interface Props extends RouteProps {
	layout?: "public" | "private";
}

export const AppRoute = ({ component: Component, layout: Layout, ...rest }: Props) => (
	<Route {...rest} render={(props: RouteComponentProps<any>) => {

		const layout = AppLayoutRegistry.get(Layout || "public");

		return React.createElement(layout, null, <Component {...props} />);
	}} />
)

export abstract class AppLayoutRegistry {
	private static Map: { [key: string]: React.ComponentType<any>; } = {};

	static register(key: string, layout: React.ComponentType<any>) {
		AppLayoutRegistry.Map[key] = layout;
	}

	static get(key: string): React.ComponentType<any> {
		return AppLayoutRegistry.Map[key];
	}
}
