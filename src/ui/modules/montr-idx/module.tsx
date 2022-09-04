import { Constants, Layout } from "@montr-core/constants";
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

	authLinkLogin: Constants.apiURL + "/authentication/linkLogin",
	authExternalLogin: Constants.apiURL + "/authentication/externalLogin",

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

	profile: "/profile/",
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

const PageRegister = React.lazy(() => import("./components/page-register"));
const PageConfirmEmail = React.lazy(() => import("./components/page-confirm-email"));
const PageConfirmEmailChange = React.lazy(() => import("./components/page-confirm-email-change"));
const PageSendEmailConfirmation = React.lazy(() => import("./components/page-send-email-confirmation"));
const PageLogin = React.lazy(() => import("./components/page-login"));
const PageLogout = React.lazy(() => import("./components/page-logout"));
const PageExternalLogin = React.lazy(() => import("./components/page-external-login"));
const PageForgotPassword = React.lazy(() => import("./components/page-forgot-password"));
const PageResetPassword = React.lazy(() => import("./components/page-reset-password"));
const PaneEditProfile = React.lazy(() => import("./components/pane-edit-profile"));
const PaneSecurity = React.lazy(() => import("./components/pane-security"));
const PaneExternalLogins = React.lazy(() => import("./components/pane-external-logins"));
const PaneExternalLoginLink = React.lazy(() => import("./components/pane-external-login-link"));

AppRouteRegistry.add(Layout.auth, [
	{ path: Patterns.register, element: <PageRegister /> },
	{ path: Patterns.confirmEmail, element: <PageConfirmEmail /> },
	{ path: Patterns.confirmEmailChange, element: <PageConfirmEmailChange /> },
	{ path: Patterns.sendEmailConfirmation, element: <PageSendEmailConfirmation /> },
	{ path: Patterns.login, element: <PageLogin /> },
	{ path: Patterns.logout, element: <PageLogout /> },
	{ path: Patterns.externalLogin, element: <PageExternalLogin /> },
	{ path: Patterns.forgotPassword, element: <PageForgotPassword /> },
	{ path: Patterns.resetPassword, element: <PageResetPassword /> },
]);

AppRouteRegistry.add(Layout.profile, [
	{ path: Patterns.profile, element: <PaneEditProfile /> },
	{ path: Patterns.profileSecurity, element: <PaneSecurity /> },
	{ path: Patterns.profileExternalLogin, element: <PaneExternalLogins /> },
	{ path: Patterns.profileExternalLoginLink, element: <PaneExternalLoginLink /> },
]);

ComponentRegistry.add([
	{ path: "@montr-idx/components/tab-edit-role-permissions", component: React.lazy(() => import("./components/tab-edit-role-permissions")) },
	{ path: "@montr-idx/components/tab-edit-user-roles", component: React.lazy(() => import("./components/tab-edit-user-roles")) },
]);
