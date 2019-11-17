import React from "react";
import { AppRouteRegistry } from "@montr-core/services/";
import { Layout } from "@montr-core/constants";

export const Patterns = {
	register: "/account/register",
	confirmEmail: "/account/confirm-email/:userId/:code",
	login: "/account/login",
	logout: "/account/logout",
	externalLogin: "/account/external-login",
	forgotPassword: "/account/forgot-password",
	resetPassword: "/account/reset-password/:code",

	profile: "/profile"
};

export const Views = {
	formLogin: "Login/Form",
	formRegister: "Register/Form",
	formExternalRegister: "ExternalRegister/Form",
	formForgotPassword: "ForgotPassword/Form",
	formResetPassword: "ResetPassword/Form",
	formChangePassword: "ChangePassword/Form",
	formSetPassword: "SetPassword/Form",

	formProfile: "Profile/Form"
};

AppRouteRegistry.add([
	{ path: Patterns.register, layout: Layout.public, exact: true, component: React.lazy(() => import("./pages/register")) },
	{ path: Patterns.confirmEmail, layout: Layout.public, exact: true, component: React.lazy(() => import("./pages/confirm-email")) },
	{ path: Patterns.login, layout: Layout.public, exact: true, component: React.lazy(() => import("./pages/login")) },
	{ path: Patterns.logout, layout: Layout.public, exact: true, component: React.lazy(() => import("./pages/logout")) },
	{ path: Patterns.externalLogin, layout: Layout.public, exact: true, component: React.lazy(() => import("./pages/external-login")) },
	{ path: Patterns.forgotPassword, layout: Layout.public, exact: true, component: React.lazy(() => import("./pages/forgot-password")) },
	{ path: Patterns.resetPassword, layout: Layout.public, exact: true, component: React.lazy(() => import("./pages/reset-password")) },

	{ path: Patterns.profile, exact: true, component: React.lazy(() => import("./pages/profile")) },
]);
