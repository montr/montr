import * as React from "react";
import {
	ArrowLeftOutlined, BarChartOutlined, CheckOutlined, CheckCircleOutlined, CheckCircleTwoTone, ClusterOutlined, DashboardOutlined, DeleteOutlined, DownOutlined, ExportOutlined, FileOutlined, FolderOutlined, HomeOutlined, Html5Outlined, Html5TwoTone, GlobalOutlined, LeftOutlined, LockOutlined, LogoutOutlined, PlusOutlined, ProjectOutlined, QuestionCircleOutlined, SelectOutlined, SettingOutlined, UserOutlined
} from "@ant-design/icons";

export abstract class Icon {
	private static Map: { [key: string]: JSX.Element; } = {};

	static get(key: string): JSX.Element {
		return Icon.Map[key];
	}

	static ArrowLeft = Icon.Map["arrow-left"] = <ArrowLeftOutlined />;
	static BarChart = Icon.Map["bar-chart"] = <BarChartOutlined />;
	static Check = Icon.Map["check"] = <CheckOutlined />;
	static CheckCircle = Icon.Map["check-circle"] = <CheckCircleOutlined />;
	static CheckCircleTwoTone = Icon.Map["check-circle-2t"] = <CheckCircleTwoTone twoToneColor="#52c41a" />;
	static Cluster = Icon.Map["cluster"] = <ClusterOutlined />;
	static Dashboard = Icon.Map["dashboard"] = <DashboardOutlined />;
	static Delete = Icon.Map["delete"] = <DeleteOutlined />;
	static Down = Icon.Map["down"] = <DownOutlined />;
	static Export = Icon.Map["export"] = <ExportOutlined />;
	static File = Icon.Map["file"] = <FileOutlined />;
	static Folder = Icon.Map["folder"] = <FolderOutlined />;
	static Home = Icon.Map["home"] = <HomeOutlined />;
	static Html5 = Icon.Map["html5"] = <Html5Outlined />;
	static Html5TwoTone = Icon.Map["html5-2t"] = <Html5TwoTone />;
	static Global = Icon.Map["global"] = <GlobalOutlined />;
	static Left = Icon.Map["left"] = <LeftOutlined />;
	static Lock = Icon.Map["lock"] = <LockOutlined />;
	static Logout = Icon.Map["login"] = <LogoutOutlined />;
	static Plus = Icon.Map["plus"] = <PlusOutlined />;
	static Project = Icon.Map["project"] = <ProjectOutlined />;
	static QuestionCircle = Icon.Map["question-circle"] = <QuestionCircleOutlined />;
	static Select = Icon.Map["select"] = <SelectOutlined />;
	static Setting = Icon.Map["setting"] = <SettingOutlined />;
	static User = Icon.Map["user"] = <UserOutlined />;
}
