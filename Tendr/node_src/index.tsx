import * as React from "react";
import * as ReactDOM from "react-dom";

import { LocaleProvider } from "antd";
import * as ru_RU from "antd/lib/locale-provider/ru_RU";

import { App as PrivateApp } from "./pages/private/App";

import "antd/dist/antd.less";
import "./index.less";

ReactDOM.render(
    <LocaleProvider locale={ru_RU as any}>
        <PrivateApp />
    </LocaleProvider >,
    document.getElementById("root"));