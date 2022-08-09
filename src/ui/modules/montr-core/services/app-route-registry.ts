import { RouteObject } from "react-router-dom";
import { IRoute } from "../models";

export abstract class AppRouteRegistry {
	private static Routes: Record<string, IRoute[]> = {};

	static add(layout: string, routes: /* IRoute[] | */ RouteObject[]) {
		const items = AppRouteRegistry.get(layout);

		Array.prototype.push.apply(items, routes);
	}

	static get(layout: string): IRoute[] {

		let items = AppRouteRegistry.Routes[layout];

		if (!items) items = AppRouteRegistry.Routes[layout] = [];

		return items;
	}
}
