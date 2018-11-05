import * as React from "react";

import { HashRouter as Router, Route } from "react-router-dom";

import { Layout, LocaleProvider, Input, message } from "antd";

import ru from "antd/lib/locale-provider/ru_RU";

import { SideMenu } from "../../components";

import { Dashboard, PrivateEvents, SelectEventTemplate } from "./";

export class App extends React.Component {
    render() {
        return (
            <Router>
                <LocaleProvider locale={ru}>
                    <Layout hasSider>
                        <SideMenu />
                        <Layout style={{ background: '#fff' }}>
                            <Layout.Header style={{ background: '#fff' }}>
                                {/*<Input.Search placeholder="input search text" onSearch={value => console.log(value)}
                                    style={{ marginTop: 20, width: 200, float: "right" }} />*/}
                            </Layout.Header>
                            <Layout.Content style={{ overflow: 'initial' }}>
                                <div style={{ padding: "0 50px 16px 50px" }}>

                                    <Route path="/" exact component={() => <Dashboard />} />
                                    <Route path="/events" component={() => <PrivateEvents />} />
                                    <Route path="/events/new" component={() => <SelectEventTemplate />} />

                                </div>
                            </Layout.Content>
                            <Layout.Footer>© {new Date().getFullYear()}</Layout.Footer>
                        </Layout>
                    </Layout>
                </LocaleProvider>
            </Router>
        );
    }
}
