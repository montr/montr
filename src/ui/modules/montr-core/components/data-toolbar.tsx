
import React from "react";
import { DataButton } from ".";
import { Button } from "../models";
import { ComponentRegistry } from "../services";

interface Props {
    buttons: Button[];
    buttonProps?: Record<string, unknown>;
}

export class DataToolbar extends React.Component<Props> {
    render = (): React.ReactNode => {
        const { buttons, buttonProps } = this.props;

        return buttons &&
            <>
                {buttons.map((button) => {
                    let component = undefined;

                    const props = { ...buttonProps, ...button.props };

                    if (button.component) {
                        const componentClass = ComponentRegistry.getComponent(button.component);

                        if (componentClass) {
                            component = React.createElement(componentClass, props);
                        } else {
                            console.error(`Component ${button.component} is not found.`);
                        }
                    } else {
                        component = <DataButton button={button} {...props} />;
                    }

                    return component;
                })}
            </>;
    };
}
