import * as React from "react";
import * as ReactDOM from "react-dom";
import { BrowserRouter, Switch } from "react-router-dom";
import { Spin, ConfigProvider } from "antd";
import ru_RU from "antd/lib/locale-provider/ru_RU";
import { CompanyContextProvider } from "@montr-kompany/components";
import { AppLayoutRegistry, ErrorBoundary, UserContextProvider, AuthCallbackHandler } from "@montr-core/components";
import { PublicLayout } from "./public-layout";
import { PrivateLayout } from "./private-layout";
import { Routes as CoreRoutes } from "@montr-core/.";
import { Routes as MasterDataRoutes } from "@montr-master-data/.";
import { Routes as PublicRoutes } from "@montr-tendr/pages/public/routes";
import { Routes as PrivateRoutes } from "@montr-tendr/pages/private/routes";

import "@montr-core/i18n";
import "@montr-core/index.less"
import "./index.less";

AppLayoutRegistry.register("public", PublicLayout);
AppLayoutRegistry.register("private", PrivateLayout);

class App extends React.Component {
	render = () => {
		return (
			<React.Suspense fallback={<Spin style={{ position: "fixed", top: "33%", left: "49%" }} />}>
				<ErrorBoundary>
					<BrowserRouter>
						<ConfigProvider locale={ru_RU}>
							<UserContextProvider>
								<CompanyContextProvider>
									<AuthCallbackHandler>

										<CoreRoutes />
										<MasterDataRoutes />
										<PublicRoutes />
										<PrivateRoutes />

									</AuthCallbackHandler>
								</CompanyContextProvider>
							</UserContextProvider>
						</ConfigProvider>
					</BrowserRouter>
				</ErrorBoundary>
			</React.Suspense>
		);
	}
}

ReactDOM.render(<App />, document.getElementById("root"));
