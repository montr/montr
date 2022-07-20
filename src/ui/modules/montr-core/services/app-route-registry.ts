import { IRoute } from "../models";

export abstract class AppRouteRegistry {
	private static Routes: IRoute[] = [];

	static add(routes: IRoute[]) {
		Array.prototype.push.apply(AppRouteRegistry.Routes, routes);
	}

	static get(layout: string): IRoute[] {
		return AppRouteRegistry.Routes.filter(x => x.layout == layout);
	}
}
