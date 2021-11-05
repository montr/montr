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
    searchTasks: "/tasks/",
    viewTask: "/tasks/view/:uid/:tabKey?",
};

export const RouteBuilder = {
    viewTask: (uid: Guid | string, tabKey?: string): string => {
        return generatePath(Patterns.viewTask, { uid: uid.toString(), tabKey });
    }
};

AppRouteRegistry.add([
    { path: Patterns.searchTasks, exact: true, component: React.lazy(() => import("./components/page-search-tasks")) },
    { path: Patterns.viewTask, exact: true, component: React.lazy(() => import("./components/page-view-task")) },
]);

ComponentRegistry.add([
    { path: "@montr-tasks/components/pane-view-task-info", component: React.lazy(() => import("./components/pane-view-task-info")) },
]);
