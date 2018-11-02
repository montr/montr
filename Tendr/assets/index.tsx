import * as React from "react";
import * as ReactDOM from "react-dom";

import { Layout, LocaleProvider } from 'antd';

import ru from 'antd/lib/locale-provider/ru_RU';

import "antd/dist/antd.css";

import { SideMenu } from "./components/SideMenu";
import { SelectEventTemplate } from "./components/SelectEventTemplate";

ReactDOM.render(
    <LocaleProvider locale={ru}>
        <Layout>
            <SideMenu />
            <Layout style={{ background: '#fff', marginLeft: 200 }}>
                <Layout.Header style={{ background: '#fff', padding: 0 }} />
                <Layout.Content style={{ margin: '16px 16px 0', overflow: 'initial' }}>
                    <div style={{ padding: '16px 8px' }}>
                        <SelectEventTemplate />
                    </div>
                </Layout.Content>
                <Layout.Footer>©2018 Tendr</Layout.Footer>
            </Layout>
        </Layout>
    </LocaleProvider>,
    document.getElementById("root")
);