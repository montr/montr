import { RouteObject } from "react-router-dom";

export abstract class AppRouteRegistry {
	private static Routes: Record<string, RouteObject[]> = {};

	static add(layout: string, routes: RouteObject[]) {
		const items = AppRouteRegistry.get(layout);

		Array.prototype.push.apply(items, routes);
	}

	static get(layout: string): RouteObject[] {

		let items = AppRouteRegistry.Routes[layout];

		if (!items) items = AppRouteRegistry.Routes[layout] = [];

		return items;
	}
}
