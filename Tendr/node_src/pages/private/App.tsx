import * as React from "react";
import { BrowserRouter as Router, Route } from "react-router-dom";

import { Layout, LocaleProvider, message } from "antd";

import ru from "antd/lib/locale-provider/ru_RU";

import { SideMenu } from "../../components";

import { Dashboard, SearchEvents, EditEvent, SelectEventTemplate } from "./";

export class App extends React.Component {

    componentDidCatch(error: Error, errorInfo: React.ErrorInfo) {
        message.error("App.componentDidCatch " + error.message);
    }

    render() {
        return (
            <Router basename="/#" >
                <LocaleProvider locale={ru}>
                    <Layout hasSider>
                        <SideMenu />
                        <Layout style={{ background: "#fff" }}>
                            <Layout.Content style={{ overflow: "initial", padding: "16px 24px 16px 24px" }}>

                                <Route path="/" exact component={() => <Dashboard />} />
                                <Route path="/events" exact component={() => <SearchEvents />} />
                                <Route path="/events/new" component={() => <SelectEventTemplate />} />
                                <Route path="/events/edit/:id"
                                    component={({ match }: any) => <EditEvent {...match} />} />

                            </Layout.Content>
                            <Layout.Footer style={{ background: "#fff" }}>© {new Date().getFullYear()}</Layout.Footer>
                        </Layout>
                    </Layout>
                </LocaleProvider>
            </Router>
        );
    }
}