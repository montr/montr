import { AppSetupRedirect, AuthCallbackHandler, ErrorBoundary, PageContextProvider, SuspenseFallback, UserContextProvider } from "@montr-core/components";
import { Layout } from "@montr-core/constants";
import { AppLayoutRegistry, AppRouteRegistry } from "@montr-core/services";
import { CompanyContextProvider } from "@montr-kompany/components";
import { ConfigProvider } from "antd";
import { Locale } from "antd/lib/locale-provider";
import en_US from "antd/lib/locale-provider/en_US";
import ru_RU from "antd/lib/locale-provider/ru_RU";
import * as React from "react";
import { createRoot } from "react-dom/client";
import { Translation } from "react-i18next";
import { BrowserRouter, Route, Routes } from "react-router-dom";
import * as Layouts from "./components";
import { AuthLayout, PrivateLayout, PublicLayout } from "./components";
import "./modules";

AppLayoutRegistry.register(Layout.auth, Layouts.AuthLayout);
AppLayoutRegistry.register(Layout.public, Layouts.PublicLayout);
AppLayoutRegistry.register(Layout.private, Layouts.PrivateLayout);

function getLocale(lng: string): Locale {
	if (lng == "ru") return ru_RU;
	return en_US;
}

const PageError404 = React.lazy(() => import("@montr-core/components/page-error-404"));

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
											{/* <AppRouteList
												routes={AppRouteRegistry.get()}
												layoutRegistry={AppLayoutRegistry.get}
												defaultLayout={Layout.private}
												errorLayout={Layout.public}
											/> */}
											<BrowserRouter>
												<AppSetupRedirect>
													<Routes>
														<Route element={<PublicLayout />} >
															{AppRouteRegistry.get(Layout.public).map(({ component: Component, ...props }, index) => {
																return <Route key={index} element={<Component />} {...props} />;
															})}
														</Route>
														<Route element={<PrivateLayout />} >
															{AppRouteRegistry.get(Layout.private).map(({ component: Component, ...props }, index) => {
																return <Route key={index} element={<Component />} {...props} />;
															})}
														</Route>
														<Route element={<AuthLayout />} >
															{AppRouteRegistry.get(Layout.auth).map(({ component: Component, ...props }, index) => {
																return <Route key={index} element={<Component />} {...props} />;
															})}
														</Route>
														<Route path="*" element={<React.Suspense fallback={<>...</>}>
															<PageError404 />
														</React.Suspense>} />
													</Routes>
												</AppSetupRedirect>
											</BrowserRouter>
										</PageContextProvider>
									</AuthCallbackHandler>
								</CompanyContextProvider>
							</UserContextProvider>
						</ConfigProvider>}
					</Translation>
				</ErrorBoundary>
			</React.Suspense >
		);
	};
}

const root = createRoot(document.getElementById("root"));

root.render(<App />);
