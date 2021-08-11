
import React from "react";
import { DataButton } from ".";
import { Button, ConfigurationItemProps } from "../models";
import { ComponentRegistry } from "../services";

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

                let component = undefined;

                if (button.component) {
                    const componentClass = ComponentRegistry.getComponent(button.component);

                    if (componentClass) {
                        component = React.createElement(componentClass, props);
                    } else {
                        console.error(`Component ${button.component} is not found.`);
                    }
                } else {
                    component = <DataButton {...props} />;
                }

                return component;
            })}
        </>;
    };
}
