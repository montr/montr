import * as React from "react";
import * as ReactDOM from "react-dom";

import { Layout, LocaleProvider } from "antd";
import { Input } from "antd";

import ru from "antd/lib/locale-provider/ru_RU";

import "antd/dist/antd.css";

import { SideMenu } from "./components/SideMenu";
import { SelectEventTemplate } from "./pages/SelectEventTemplate";

ReactDOM.render(
    <LocaleProvider locale={ru}>
        <Layout>
            <SideMenu />
            <Layout style={{ background: '#fff', marginLeft: 200 }}>
                <Layout.Header style={{ background: '#fff' }}>
                    <Input.Search placeholder="input search text" onSearch={value => console.log(value)}
                        style={{ width: 200, float: "right" }} />
                    <h2>Выберите шаблон</h2>
                </Layout.Header>
                <Layout.Content style={{ overflow: 'initial' }}>
                    <div style={{ padding: "16px 50px" }}>
                        <SelectEventTemplate />
                    </div>
                </Layout.Content>
                <Layout.Footer>©{new Date().getFullYear()}</Layout.Footer>
            </Layout>
        </Layout>
    </LocaleProvider>,
    document.getElementById("root")
);