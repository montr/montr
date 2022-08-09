import { Layout } from "@montr-core/constants";
import { AppRouteRegistry, ComponentRegistry } from "@montr-core/services/";
import React from "react";

export const Locale = {
	Namespace: "idx"
};

export const StorageNames = {
	email: "email"
};

export const Api = {
	accountRegister: "/account/register",
	accountSendEmailConfirmation: "/account/sendEmailConfirmation",
	accountConfirmEmail: "/account/confirmEmail",
	accountConfirmEmailChange: "/account/confirmEmailChange",
	accountLogin: "/account/login",
	accountLogout: "/account/logout",
	accountExternalLoginCallback: "/account/externalLoginCallback",
	accountExternalRegister: "/account/externalRegister",
	accountAuthSchemes: "/account/authSchemes",
	accountForgotPassword: "/account/forgotPassword",
	accountResetPassword: "/account/resetPassword",

	profileGet: "/profile/get",
	profileUpdate: "/profile/update",
	profileChangeEmail: "/profile/changeEmail",
	profileChangePhone: "/profile/changePhone",
	profileChangePassword: "/profile/changePassword",
	profileSetPassword: "/profile/setPassword",
	profileExternalLogins: "/profile/externalLogins",
	profileLinkLoginCallback: "/profile/linkLoginCallback",
	profileRemoveLogin: "/profile/removeLogin",

	authLinkLogin: "/authentication/linkLogin",
	authExternalLogin: "/authentication/externalLogin",

	rolePermissionList: "/rolePermission/list",
	rolePermissionUpdate: "/rolePermission/update",

	userRoleList: "/userRole/listRoles",
	userRoleAddRoles: "/userRole/addRoles",
	userRoleRemoveRoles: "/userRole/removeRoles",
};

export const Patterns = {
	register: "/account/register",
	confirmEmail: "/account/confirm-email/:userId/:code",
	confirmEmailChange: "/account/confirm-email-change/:userId/:email/:code",
	sendEmailConfirmation: "/account/send-email-confirmation",
	login: "/account/login",
	logout: "/account/logout",
	externalLogin: "/account/external-login",
	forgotPassword: "/account/forgot-password",
	resetPassword: "/account/reset-password/:code",

	profile: "/profile",
	profileSecurity: "/profile/security",
	profileExternalLogin: "/profile/external-login",
	profileExternalLoginLink: "/profile/external-login/link"
};

export const Views = {
	formSendEmailConfirmation: "SendEmailConfirmation/Form",
	formLogin: "Login/Form",
	formRegister: "Register/Form",
	formExternalRegister: "ExternalRegister/Form",
	formUpdateProfile: "UpdateProfile/Form",
	formChangeEmail: "ChangeEmail/Form",
	formChangePhone: "ChangePhone/Form",
	formForgotPassword: "ForgotPassword/Form",
	formResetPassword: "ResetPassword/Form",
	formChangePassword: "ChangePassword/Form",
	formSetPassword: "SetPassword/Form",

	rolePermissionsGrid: "RolePermissions/Grid",

	userRolesGrid: "UserRoles/Grid",
};

AppRouteRegistry.add(Layout.auth, [
	{ path: Patterns.register, component: React.lazy(() => import("./components/page-register")) },
	{ path: Patterns.confirmEmail, component: React.lazy(() => import("./components/page-confirm-email")) },
	{ path: Patterns.confirmEmailChange, component: React.lazy(() => import("./components/page-confirm-email-change")) },
	{ path: Patterns.sendEmailConfirmation, component: React.lazy(() => import("./components/page-send-email-confirmation")) },
	{ path: Patterns.login, component: React.lazy(() => import("./components/page-login")) },
	{ path: Patterns.logout, component: React.lazy(() => import("./components/page-logout")) },
	{ path: Patterns.externalLogin, component: React.lazy(() => import("./components/page-external-login")) },
	{ path: Patterns.forgotPassword, component: React.lazy(() => import("./components/page-forgot-password")) },
	{ path: Patterns.resetPassword, component: React.lazy(() => import("./components/page-reset-password")) },
]);

/* AppRouteRegistry.add(Layout.private, [
	{ path: Patterns.profile, component: React.lazy(() => import("./components/page-profile")) }
]); */

AppRouteRegistry.add(Layout.profile, [
	{ path: Patterns.profile, component: React.lazy(() => import("./components/pane-edit-profile")) },
	{ path: Patterns.profileSecurity, component: React.lazy(() => import("./components/pane-security")) },
	{ path: Patterns.profileExternalLogin, component: React.lazy(() => import("./components/pane-external-logins")) },
	{ path: Patterns.profileExternalLoginLink, component: React.lazy(() => import("./components/pane-external-login-link")) },
]);

ComponentRegistry.add([
	{ path: "@montr-idx/components/tab-edit-role-permissions", component: React.lazy(() => import("./components/tab-edit-role-permissions")) },
	{ path: "@montr-idx/components/tab-edit-user-roles", component: React.lazy(() => import("./components/tab-edit-user-roles")) },
]);
