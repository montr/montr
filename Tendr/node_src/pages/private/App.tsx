import * as React from "react";
import { BrowserRouter as Router, Route } from "react-router-dom";

import { Layout, LocaleProvider, Input, message } from "antd";

import ru from "antd/lib/locale-provider/ru_RU";

import { SideMenu } from "../../components";

import { Dashboard, SearchEvents, CreateEvent, SelectEventTemplate } from "./";

export class App extends React.Component {
    render() {
        return (
            <Router basename="/#" >
                <LocaleProvider locale={ru}>
                    <Layout hasSider>
                        <SideMenu />
                        <Layout style={{ background: "#fff" }}>
                            <Layout.Header style={{ background: "#fff" }}>
                                {/*<Input.Search placeholder="input search text" onSearch={value => console.log(value)}
                                    style={{ marginTop: 20, width: 200, float: "right" }} />*/}
                            </Layout.Header>
                            <Layout.Content style={{ overflow: "initial" }}>


                                <Route path="/" exact component={() => <Dashboard />} />
                                <Route path="/events" exact component={() => <SearchEvents />} />
                                <Route path="/events/new" component={() => <SelectEventTemplate />} />

                            </Layout.Content>
                            <Layout.Footer style={{ background: "#fff" }}>© {new Date().getFullYear()}</Layout.Footer>
                        </Layout>
                    </Layout>
                </LocaleProvider>
            </Router>
        );
    }
}
