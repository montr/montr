import * as React from "react";
import {
	ArrowDownOutlined, ArrowLeftOutlined, ArrowUpOutlined, BarChartOutlined, CheckOutlined, CheckCircleOutlined, CheckCircleTwoTone, ClusterOutlined, ContainerOutlined, DashboardOutlined, DeleteOutlined, DownOutlined, EditOutlined, ExportOutlined, FacebookOutlined, FileOutlined, FolderOutlined, GoogleOutlined, HomeOutlined, Html5Outlined, Html5TwoTone, ImportOutlined, GlobalOutlined, LeftOutlined, LockOutlined, LogoutOutlined, MinusCircleOutlined, PlusOutlined, ProjectOutlined, QuestionCircleOutlined, SearchOutlined, SelectOutlined, SettingOutlined, UserOutlined, WindowsOutlined
} from "@ant-design/icons";

export abstract class Icon {
	private static Map: { [key: string]: JSX.Element; } = {};

	static get(key: string): JSX.Element {
		return Icon.Map[key];
	}

	static ArrowDown = Icon.Map["arrow-down"] = <ArrowDownOutlined />;
	static ArrowLeft = Icon.Map["arrow-left"] = <ArrowLeftOutlined />;
	static ArrowUp = Icon.Map["arrow-up"] = <ArrowUpOutlined />;
	static BarChart = Icon.Map["bar-chart"] = <BarChartOutlined />;
	static Check = Icon.Map["check"] = <CheckOutlined />;
	static CheckCircle = Icon.Map["check-circle"] = <CheckCircleOutlined />;
	static CheckCircleTwoTone = Icon.Map["check-circle-2t"] = <CheckCircleTwoTone twoToneColor="#52c41a" />;
	static Cluster = Icon.Map["cluster"] = <ClusterOutlined />;
	static Container = Icon.Map["container"] = <ContainerOutlined />;
	static Dashboard = Icon.Map["dashboard"] = <DashboardOutlined />;
	static Delete = Icon.Map["delete"] = <DeleteOutlined />;
	static Down = Icon.Map["down"] = <DownOutlined />;
	static Edit = Icon.Map["edit"] = <EditOutlined />;
	static Export = Icon.Map["export"] = <ExportOutlined />;
	static Facebook = Icon.Map["facebook"] = <FacebookOutlined />;
	static File = Icon.Map["file"] = <FileOutlined />;
	static Folder = Icon.Map["folder"] = <FolderOutlined />;
	static Google = Icon.Map["google"] = <GoogleOutlined />;
	static Home = Icon.Map["home"] = <HomeOutlined />;
	static Html5 = Icon.Map["html5"] = <Html5Outlined />;
	static Html5TwoTone = Icon.Map["html5-2t"] = <Html5TwoTone />;
	static Import = Icon.Map["import"] = <ImportOutlined />;
	static Global = Icon.Map["global"] = <GlobalOutlined />;
	static Left = Icon.Map["left"] = <LeftOutlined />;
	static Lock = Icon.Map["lock"] = <LockOutlined />;
	static Logout = Icon.Map["login"] = <LogoutOutlined />;
	static MinusCircle = Icon.Map["minus-circle"] = <MinusCircleOutlined />;
	static Plus = Icon.Map["plus"] = <PlusOutlined />;
	static Project = Icon.Map["project"] = <ProjectOutlined />;
	static QuestionCircle = Icon.Map["question-circle"] = <QuestionCircleOutlined />;
	static Search = Icon.Map["search"] = <SearchOutlined />;
	static Select = Icon.Map["select"] = <SelectOutlined />;
	static Setting = Icon.Map["setting"] = <SettingOutlined />;
	static User = Icon.Map["user"] = <UserOutlined />;
	static Windows = Icon.Map["windows"] = <WindowsOutlined />;
}
