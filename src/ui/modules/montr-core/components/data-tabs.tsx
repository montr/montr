import React from "react";
import { Tabs } from "antd";
import { DataPane } from "../models";
import { ComponentRegistry } from "../services";
import { Icon } from ".";

interface Props<TModel> {
    tabKey: string;
    panes: DataPane<TModel>[],
    onTabChange?: (tabKey: string) => void,
    disabled?: (pane: DataPane<TModel>, index: number) => boolean,
    tabProps?: any;
}

export class DataTabs<TModel> extends React.Component<Props<TModel>> {

    render = () => {
        const { tabKey, panes, onTabChange, disabled, tabProps } = this.props;

        return (<>
            {panes &&
                <Tabs size="small" defaultActiveKey={tabKey} onChange={onTabChange}>
                    {panes.map((pane, index) => {

                        let component: React.ReactElement;

                        if (pane.component) {
                            const componentClass = ComponentRegistry.getComponent(pane.component);

                            component = React.createElement(componentClass, tabProps);
                        }

                        return (
                            <Tabs.TabPane key={pane.key}
                                tab={<span>{pane.icon && Icon.get(pane.icon)} {pane.name}</span>}
                                disabled={disabled ? disabled(pane, index) : false}>
                                {component}
                            </Tabs.TabPane>
                        );
                    })}
                </Tabs>
            }
        </>);
    };
}
