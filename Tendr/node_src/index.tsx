import * as React from "react";
import * as ReactDOM from "react-dom";

import { App as PrivateApp } from "./pages/private/App";

import "antd/dist/antd.css";

ReactDOM.render(
    <PrivateApp />,
    document.getElementById("root")
);