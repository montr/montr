interface ComponentInfo {
	path: string,
	component?: React.ComponentType<any>;
}

export abstract class ComponentRegistry {
	static Components: ComponentInfo[] = [];

	static add(items: ComponentInfo[]): void {
		Array.prototype.push.apply(ComponentRegistry.Components, items);
	}

	static getComponent(path: string): React.ComponentType<any> {
		return ComponentRegistry.Components.find(x => x.path == path)?.component;
	}
}

export abstract class ComponentNameConvention {
	static entityPane(entityTypeCode: string): string {
		return `@montr/entity-pane/${entityTypeCode}`;
	}
}
