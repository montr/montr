import React from "react";
import { History } from "history";
import { Tabs } from "antd";
import { Pane, PaneProps } from "../models";
import { Icon } from ".";

interface Props<TModel> {
    tabKey: string;
    panes: Pane<TModel>[],
    buildRoute: (tabKey: string) => string,
    history: History,
    data: TModel
}

export class DataTabs<TModel> extends React.Component<Props<TModel>> {

    handleTabChange = (tabKey: string) => {
        const { buildRoute } = this.props;

        const path = buildRoute(tabKey);

        this.props.history.replace(path);
    };

    render = () => {
        const { tabKey, panes, data } = this.props;

        return (<>
            {panes &&
                <Tabs size="small" defaultActiveKey={tabKey} onChange={this.handleTabChange}>
                    {panes.map(pane => {

                        let component: React.ReactElement<PaneProps<TModel>>;
                        if (pane.component) {
                            component = React.createElement(pane.component,
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
    }
}
