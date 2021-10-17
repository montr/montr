import { AppRouteRegistry, ComponentRegistry } from "@montr-core/services";
import React from "react";

export const Locale = {
    Namespace: "tasks"
};

export const Api = {
    taskList: "/task/list",
};

export const Views = {
    taskList: "task-list",
};

export const Patterns = {
    searchTask: "/tasks/",
};

export const RouteBuilder = {
};

AppRouteRegistry.add([
    { path: Patterns.searchTask, exact: true, component: React.lazy(() => import("./components/page-search-tasks")) },
]);

ComponentRegistry.add([

]);
