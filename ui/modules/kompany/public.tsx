import * as React from "react";
import * as ReactDOM from "react-dom";

import { App } from "./pages/public/App";
// import { AuthService } from "@montr-core/services/AuthService"

import "./index.less";

// new AuthService().loginSilent();

ReactDOM.render(<App />, document.getElementById("root"));
