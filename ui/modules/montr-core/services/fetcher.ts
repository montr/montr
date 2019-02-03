import axios from "axios"

import { AuthService } from "./"

const authService = new AuthService();
const authenticated = axios.create();

authenticated.interceptors.request.use(
	async (config) => {

		const user = await authService.getUser();

		if (user && user.access_token) {
			config.headers.Authorization = `Bearer ${user.access_token}`;
		}

		return config;
	}, (error) => {

		/* if (axios.isCancel(error)) {
			return;
		} */

		return Promise.reject(error);
	}
);

authenticated.interceptors.response.use(null,
	(error) => {

		/*  */

		if (error.response && error.response.status === 401) {
			authService.login();
			return;
		}

		return Promise.reject(error);
	}
);

export class Fetcher {

	private _cancelTokenSource = axios.CancelToken.source();

	public post = async (url: string, body?: any): Promise<any> => {

		const response = await authenticated
			.post(url, body, { cancelToken: this._cancelTokenSource.token });

		return response ? response.data : null;
	}

	public abort = async (message?: string): Promise<any> => {
		this._cancelTokenSource.cancel(message || `${this.constructor.name} cancelled`);
	}
}
