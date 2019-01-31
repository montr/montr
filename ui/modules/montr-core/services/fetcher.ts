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
		return Promise.reject(error);
	});

authenticated.interceptors.response.use(null,
	(error) => {
		if (error.response && error.response.status === 401) {
			authService.login();
			return;
		}

		return Promise.reject(error);
	});

export class Fetcher {
	public async post(url: string, body?: any): Promise<any> {

		const response = await authenticated.post(url, body);

		return response ? response.data : null;
	}
}
