import * as React from 'react';

import { Link } from "react-router-dom";

import { Layout, Menu, Icon } from 'antd';

export class SideMenu extends React.Component {
    public render() {
        return (
            <Layout.Sider theme="dark" breakpoint="lg" collapsedWidth="0"
                style={{ height: '100vh' }}>
                <div className="logo" />
                <Menu theme="dark" mode="inline">
                    <Menu.Item key="1">
                        <Link to="/">
                            <Icon type="dashboard" />
                            <span className="nav-text">Панель управления</span>
                        </Link>
                    </Menu.Item>
                    <Menu.Item key="2">
                        <Link to="/events">
                            <Icon type="project" />
                            <span className="nav-text">Торговые процедуры</span>
                        </Link>
                    </Menu.Item>
                </Menu>
            </Layout.Sider>
        );
    }
};