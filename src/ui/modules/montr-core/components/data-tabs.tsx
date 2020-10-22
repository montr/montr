import React from "react";
import { Tabs } from "antd";
import { DataPane, PaneProps } from "../models";
import { ComponentRegistry } from "../services";
import { Icon } from ".";

interface Props<TModel> {
    tabKey: string;
    panes: DataPane<TModel>[],
    onTabChange?: (tabKey: string) => void,
    data: TModel;
}

export class DataTabs<TModel> extends React.Component<Props<TModel>> {

    render = () => {
        const { tabKey, panes, onTabChange, data } = this.props;

        return (<>
            {panes &&
                <Tabs size="small" defaultActiveKey={tabKey} onChange={onTabChange}>
                    {panes.map(pane => {

                        let component: React.ReactElement<PaneProps<TModel>>;

                        if (pane.component) {
                            const componentClass = ComponentRegistry.getComponent(pane.component);

                            component = React.createElement(componentClass,
                                { data: data /* , ref: this.createRefForKey(pane.key) */ });
                        }

                        return (
                            <Tabs.TabPane key={pane.key}
                                tab={<span>{pane.icon && Icon.get(pane.icon)} {pane.name}</span>}>

                                {component}

                            </Tabs.TabPane>
                        );
                    })}
                </Tabs>
            }
        </>);
    };
}
