import * as React from 'react';

import { Link } from "react-router-dom";

import { Layout, Menu, Icon } from 'antd';

export class SideMenu extends React.Component {
    public render() {
        return (
            <Layout.Sider theme="dark" breakpoint="md"
                style={{ overflow: 'auto', height: '100vh', position: 'fixed', left: 0 }}>
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
                    <Menu.Item key="4">
                        <Link to="/selectEventTemplate">
                            <Icon type="heart" />
                            <span className="nav-text">Выбор шаблона</span>
                        </Link>
                    </Menu.Item>
                </Menu>
            </Layout.Sider>
        );
    }
};