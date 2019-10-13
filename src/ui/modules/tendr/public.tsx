import * as React from "react";
import * as ReactDOM from "react-dom";
import { App } from "./pages/public";

import "./i18n";

import "@montr-core/index.less"
import "./index.less";

ReactDOM.render(<App />, document.getElementById("root"));
