
import React from "react";
import { Button, ConfigurationItemProps } from "../models";
import { ComponentFactory } from "./component-factory";
import { DataButton } from "./data-button";
import { Toolbar } from "./toolbar";

interface Props {
	buttons: Button[];
	buttonProps?: ConfigurationItemProps;
}

export class DataToolbar extends React.Component<Props> {
	render = (): React.ReactNode => {
		const { buttons, buttonProps } = this.props;

		if (!buttons) return null;

		return <Toolbar>
			{buttons.map((button, index) => {

				const key = /* button.key || */ `button-${index}`;

				const props = { key, button, ...buttonProps, ...button.props };

				return ComponentFactory.createComponent(button.component, props)
					?? <DataButton {...props} />;
			})}
		</Toolbar>;
	};
}
