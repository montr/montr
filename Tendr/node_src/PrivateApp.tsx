import * as React from "react";

import { BrowserRouter as Router, Route } from "react-router-dom";

import { Layout, LocaleProvider } from "antd";
import { Input } from "antd";

import ru from "antd/lib/locale-provider/ru_RU";

import "antd/dist/antd.css";

import { SideMenu } from "./components/SideMenu";

import { Hello } from "./pages/Hello";
import { PrivateEvents } from "./pages/PrivateEvents";
import { SelectEventTemplate } from "./pages/SelectEventTemplate";

export class PrivateApp extends React.Component {
    render() {
        return (
            <Router basename="#">
                <LocaleProvider locale={ru}>
                    <Layout hasSider>
                        <SideMenu />
                        <Layout style={{ background: '#fff', marginLeft: 200 }}>
                            <Layout.Header style={{ background: '#fff' }}>
                                <Input.Search placeholder="input search text" onSearch={value => console.log(value)}
                                    style={{ marginTop: 20, width: 200, float: "right" }} />
                            </Layout.Header>
                            <Layout.Content style={{ overflow: 'initial' }}>
                                <div style={{ padding: "16px 50px" }}>

                                    <Route path="/" exact component={() => <Hello compiler="TypeScript" framework="React" />} />
                                    <Route path="/events" component={() => <PrivateEvents />} />
                                    <Route path="/selectEventTemplate" component={() => <SelectEventTemplate />} />

                                </div>
                            </Layout.Content>
                            <Layout.Footer>©{new Date().getFullYear()}</Layout.Footer>
                        </Layout>
                    </Layout>
                </LocaleProvider>
            </Router>
        );
    }
}
