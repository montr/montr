import * as React from "react";
import * as ReactDOM from "react-dom";

import { App as PrivateApp } from "./pages/private/App";

import "antd/dist/antd.less";
import "./index.less";

ReactDOM.render(
    <PrivateApp />,
    document.getElementById("root")
);