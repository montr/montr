export abstract class AppLayoutRegistry {
	private static Map: { [key: string]: React.ComponentType<any>; } = {};

	static register(key: string, layout: React.ComponentType<any>) {
		AppLayoutRegistry.Map[key] = layout;
	}

	static get(key: string): React.ComponentType<any> {
		return AppLayoutRegistry.Map[key];
	}
}
