import { ErrorBoundary, PageContextProvider, SuspenseFallback, UserContextProvider } from "@montr-core/components";
import { AppSetupRedirect } from "@montr-core/components/app-setup-redirect";
import { AuthCallbackHandler } from "@montr-core/components/auth-callback-handler";
import { CompanyContextProvider } from "@montr-kompany/components/company-context-provider";
import { ConfigProvider } from "antd";
import { Locale } from "antd/lib/locale-provider";
import en_US from "antd/lib/locale-provider/en_US";
import ru_RU from "antd/lib/locale-provider/ru_RU";
import * as React from "react";
import { createRoot } from "react-dom/client";
import { Translation } from "react-i18next";
import { BrowserRouter } from "react-router-dom";
import { AppRoutes } from "./app-routes";
import "./modules";

function getLocale(lng: string): Locale {
	if (lng == "ru") return ru_RU;
	return en_US;
}
class App extends React.Component {
	render = () => {
		return (
			<React.Suspense fallback={<SuspenseFallback />}>
				<ErrorBoundary>
					<Translation>{(t, { i18n, lng }) =>
						<ConfigProvider locale={getLocale(lng)}>
							<UserContextProvider>
								<CompanyContextProvider>
									<AuthCallbackHandler>
										<PageContextProvider>
											<BrowserRouter>
												<AppSetupRedirect>
													<AppRoutes />
												</AppSetupRedirect>
											</BrowserRouter>
										</PageContextProvider>
									</AuthCallbackHandler>
								</CompanyContextProvider>
							</UserContextProvider>
						</ConfigProvider>}
					</Translation>
				</ErrorBoundary>
			</React.Suspense>
		);
	};
}

const root = createRoot(document.getElementById("root"));

root.render(<App />);
