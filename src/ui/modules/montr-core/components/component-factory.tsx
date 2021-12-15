import React from "react";
import { ComponentRegistry } from "../services";

export abstract class ComponentFactory {
	public static createComponent(type: string, props: unknown) {
		let component = undefined;

		if (type) {
			const componentClass = ComponentRegistry.getComponent(type);

			if (componentClass) {
				component = React.createElement(componentClass, props);
			} else {
				console.warn(`Warning: Component '${type}' is not found.`);
			}
		}

		return component;
	}
}
