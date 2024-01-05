export interface AppOptions {
	appUrl: string;
	authorityAppUrl?: string;
	cookieDomain: string;
	state: AppState;
}

export enum AppState {
	None = "None",
	Initialized = "Initialized"
}
