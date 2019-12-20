import * as React from "react";
import * as icons from "@ant-design/icons";

export abstract class Icons {
	private static Map: { [key: string]: JSX.Element; } = {};

	static register(key: string, icon: JSX.Element) {
		Icons.Map[key] = icon;
	}

	static get(key: string): JSX.Element {
		return Icons.Map[key];
	}
}

Icons.register("arrow-left", <icons.ArrowLeftOutlined />);
Icons.register("bar-chart", <icons.BarChartOutlined />);
Icons.register("dashboard", <icons.DashboardOutlined />);
Icons.register("down", <icons.DownOutlined />);
Icons.register("home", <icons.HomeOutlined />);
Icons.register("html5", <icons.Html5Outlined />);
Icons.register("html5-2t", <icons.Html5TwoTone />);
Icons.register("global", <icons.GlobalOutlined />);
Icons.register("left", <icons.LeftOutlined />);
Icons.register("lock", <icons.LockOutlined />);
Icons.register("login", <icons.LogoutOutlined />);
Icons.register("project", <icons.ProjectOutlined />);
Icons.register("setting", <icons.SettingOutlined />);
Icons.register("user", <icons.UserOutlined />);
