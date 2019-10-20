import { IRoute } from "../models";

export abstract class AppRouteRegistry {
	static Routes: IRoute[] = [];

	static add(routes: IRoute[]) {
		Array.prototype.push.apply(AppRouteRegistry.Routes, routes);
	}

	static get(): IRoute[] {
		return AppRouteRegistry.Routes;
	}
}
