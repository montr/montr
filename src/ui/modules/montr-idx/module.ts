import React from "react";
import { AppRouteRegistry, ComponentRegistry } from "@montr-core/services/";
import { Layout, Constants } from "@montr-core/constants";

export const Locale = {
	Namespace: "idx"
};

export const StorageNames = {
	email: "email"
};

export const Api = {
	authLinkLogin: `${Constants.apiURL}/authentication/linkLogin`,
	authExternalLogin: `${Constants.apiURL}/authentication/externalLogin`,

	userCreate: `${Constants.apiURL}/user/create`,
	userGet: `${Constants.apiURL}/user/get`,
	userList: `${Constants.apiURL}/user/list`,
	userInsert: `${Constants.apiURL}/user/insert`,
	userUpdate: `${Constants.apiURL}/user/update`,
	userDelete: `${Constants.apiURL}/user/delete`,
	userRoleList: `${Constants.apiURL}/userRole/listRoles`,

	roleCreate: `${Constants.apiURL}/role/create`,
	roleGet: `${Constants.apiURL}/role/get`,
	roleList: `${Constants.apiURL}/role/list`,
	roleInsert: `${Constants.apiURL}/role/insert`,
	roleUpdate: `${Constants.apiURL}/role/update`,
	roleDelete: `${Constants.apiURL}/role/delete`,
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
	profileExternalLoginLink: "/profile/external-login/link",

	searchUsers: "/users",
	addUser: "/users/new",
	editUser: "/users/edit/:uid/:tabKey?",

	searchRoles: "/roles",
	addRole: "/roles/new",
	editRole: "/roles/edit/:uid/:tabKey?",
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

	gridSearchUsers: "UserSearch/Grid",
	userEdit: "User/Edit",
	userRolesGrid: "UserRoles/Grid",

	gridSearchRoles: "RoleSearch/Grid",
	roleEdit: "Role/Edit",
};

AppRouteRegistry.add([
	{ path: Patterns.register, layout: Layout.auth, exact: true, component: React.lazy(() => import("./components/page-register")) },
	{ path: Patterns.confirmEmail, layout: Layout.auth, exact: true, component: React.lazy(() => import("./components/page-confirm-email")) },
	{ path: Patterns.confirmEmailChange, layout: Layout.auth, exact: true, component: React.lazy(() => import("./components/page-confirm-email-change")) },
	{ path: Patterns.sendEmailConfirmation, layout: Layout.auth, exact: true, component: React.lazy(() => import("./components/page-send-email-confirmation")) },
	{ path: Patterns.login, layout: Layout.auth, exact: true, component: React.lazy(() => import("./components/page-login")) },
	{ path: Patterns.logout, layout: Layout.auth, exact: true, component: React.lazy(() => import("./components/page-logout")) },
	{ path: Patterns.externalLogin, layout: Layout.auth, exact: true, component: React.lazy(() => import("./components/page-external-login")) },
	{ path: Patterns.forgotPassword, layout: Layout.auth, exact: true, component: React.lazy(() => import("./components/page-forgot-password")) },
	{ path: Patterns.resetPassword, layout: Layout.auth, exact: true, component: React.lazy(() => import("./components/page-reset-password")) },

	{ path: Patterns.profile, component: React.lazy(() => import("./components/page-profile")) },

	{ path: Patterns.searchUsers, exact: true, component: React.lazy(() => import("./components/page-search-users")) },
	{ path: Patterns.editUser, exact: true, component: React.lazy(() => import("./components/page-edit-user")) },

	{ path: Patterns.searchRoles, exact: true, component: React.lazy(() => import("./components/page-search-roles")) },
	{ path: Patterns.editRole, exact: true, component: React.lazy(() => import("./components/page-edit-role")) }
]);

export const ProfileRoutes = [
	{ path: Patterns.profile, exact: true, component: React.lazy(() => import("./components/pane-edit-profile")) },
	{ path: Patterns.profileSecurity, exact: true, component: React.lazy(() => import("./components/pane-security")) },
	{ path: Patterns.profileExternalLogin, exact: true, component: React.lazy(() => import("./components/pane-external-logins")) },
	{ path: Patterns.profileExternalLoginLink, exact: true, component: React.lazy(() => import("./components/pane-external-login-link")) },
];

ComponentRegistry.add([
	{ path: "components/tab-edit-role", component: React.lazy(() => import("./components/tab-edit-role")) },
	{ path: "components/tab-edit-user", component: React.lazy(() => import("./components/tab-edit-user")) },
	{ path: "components/tab-edit-user-roles", component: React.lazy(() => import("./components/tab-edit-user-roles")) },
]);
