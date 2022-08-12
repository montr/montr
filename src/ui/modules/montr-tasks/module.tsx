import { Layout } from "@montr-core/constants";
import { Guid } from "@montr-core/models";
import { AppRouteRegistry, ComponentRegistry } from "@montr-core/services";
import React from "react";
import { generatePath } from "react-router";

export const EntityTypeCode = {
	task: "task"
};

export const Locale = {
	Namespace: "tasks"
};

export const Api = {
	taskMetadata: "/task/metadata",
	taskList: "/task/list",
	taskGet: "/task/get",
	taskUpdate: "/task/update",
};

export const Views = {
	taskList: "task-list",
	taskPage: "task-page",
	taskForm: "task-form",
};

export const Patterns = {
	searchTasks: "/tasks",
	viewTask: "/tasks/view/:uid",
	viewTaskTab: "/tasks/view/:uid/:tabKey",
};

export const RouteBuilder = {
	viewTask: (uid: Guid | string, tabKey = "info"): string => {
		return generatePath(Patterns.viewTaskTab, { uid: uid.toString(), tabKey });
	}
};

const PageSearchTasks = React.lazy(() => import("./components/page-search-tasks"));
const PageViewTask = React.lazy(() => import("./components/page-view-task"));

AppRouteRegistry.add(Layout.private, [
	{ path: Patterns.searchTasks, element: <PageSearchTasks /> },
	{ path: Patterns.viewTask, element: <PageViewTask /> },
	{ path: Patterns.viewTaskTab, element: <PageViewTask /> }
]);

ComponentRegistry.add([
	{ path: "@montr-tasks/components/pane-view-task-info", component: React.lazy(() => import("./components/pane-view-task-info")) },
]);
