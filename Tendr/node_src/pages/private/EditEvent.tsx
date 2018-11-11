import * as React from "react";

import { Tabs } from "antd";
import { FormComponentProps } from "antd/lib/form";

import { IEvent, EventAPI } from "../../api";
import { Page } from "../../components/";

interface EditEventProps {
    params: {
        id: number
    };
}

export class EditEvent extends React.Component<EditEventProps, {}> {

    render() {
        const data = {
            name: "Процедура",
        }

        function callback(key: any) {
            console.log(key);
        }

        return (
            <Page title={`№${this.props.params.id}`}>

                <Tabs defaultActiveKey="1" onChange={callback}>
                    <Tabs.TabPane tab="Tab 1" key="1">Content of Tab Pane 1</Tabs.TabPane>
                    <Tabs.TabPane tab="Tab 2" key="2">Content of Tab Pane 2</Tabs.TabPane>
                    <Tabs.TabPane tab="Tab 3" key="3">Content of Tab Pane 3</Tabs.TabPane>
                </Tabs>
            </Page>
        );
    }
}