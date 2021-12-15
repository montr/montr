
import React from "react";
import { DataButton } from ".";
import { Button, ConfigurationItemProps } from "../models";
import { ComponentFactory } from "./component-factory";

interface Props {
	buttons: Button[];
	buttonProps?: ConfigurationItemProps;
}

export class DataToolbar extends React.Component<Props> {
	render = (): React.ReactNode => {
		const { buttons, buttonProps } = this.props;

		if (!buttons) return null;

		return <>
			{buttons.map((button, index) => {

				const key = button.key || `btn-${index}`;

				const props = { key, button, ...buttonProps, ...button.props };

				return ComponentFactory.createComponent(button.component, props)
					?? <DataButton {...props} />;
			})}
		</>;
	};
}
