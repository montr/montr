import axios, { AxiosRequestConfig, AxiosResponse } from "axios"
import { AuthService } from "./auth-service"

// axios.defaults.withCredentials = true;
axios.defaults.headers.common["X-Requested-With"] = "XMLHttpRequest";
axios.defaults.headers.common["Access-Control-Allow-Origin"] = "*";

const authService = new AuthService();
const authenticated = axios.create({
	// withCredentials: true,
});

authenticated.interceptors.request.use(
	async (config) => {

		const user = await authService.getUser();

		if (user && user.access_token) {
			// config.headers.Authorization = `Bearer ${user.access_token}`;
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

		if (error.response && error.response.status === 401) {
			authService.login();
			return;
		}

		return Promise.reject(error);
	}
);

export class Fetcher {

	private _cancelTokenSource = axios.CancelToken.source();

	private getRequestConfig(): AxiosRequestConfig {

		const config: AxiosRequestConfig = {
			cancelToken: this._cancelTokenSource.token,
		};

		// https://github.com/axios/axios/issues/191#issuecomment-311069164
		config.withCredentials = true;

		return config;
	}

	public download = async (url: string, body?: any): Promise<any> => {

		const config = this.getRequestConfig();

		config.responseType = "blob";

		const response = await authenticated.post(url, body, config);

		this.openFile(response);
	}

	public post = async (url: string, body?: any): Promise<any> => {

		const config = this.getRequestConfig();

		// const response = await authenticated.post(url, JSON.stringify(body), config);
		const response = await authenticated.post(url, body || {}, config);

		return response ? response.data : null;
	}

	public abort = async (message?: string): Promise<any> => {
		this._cancelTokenSource.cancel(message || `${this.constructor.name} cancelled`);
	}

	private openFile(response: AxiosResponse<any>) {

		// https://stackoverflow.com/questions/16086162/handle-file-download-from-ajax-post/23797348#23797348
		// see also: https://github.com/kennethjiang/js-file-download
		// see also: https://github.com/eligrey/FileSaver.js/

		if (response.status == 200) {

			var filename = "";
			var disposition = response.headers["content-disposition"];
			if (disposition && disposition.indexOf("attachment") !== -1) {
				var filenameRegex = /filename[^;=\n]*=((['"]).*?\2|[^;\n]*)/;
				var matches = filenameRegex.exec(disposition);
				if (matches != null && matches[1]) filename = matches[1].replace(/['"]/g, '');
			}

			var type = response.headers["content-type"] || "application/octet-stream";

			var blob = typeof File === "function"
				? new File([response.data], filename, { type: type })
				: new Blob([response.data], { type: type });

			if (typeof window.navigator.msSaveBlob !== 'undefined') {
				// IE workaround for "HTML7007: One or more blob URLs were revoked by closing the blob for which they were created. These URLs will no longer resolve as the data backing the URL has been freed."
				window.navigator.msSaveBlob(blob, filename);
			} else {
				var URL = window.URL /* || window.webkitURL */;
				var downloadUrl = URL.createObjectURL(blob);

				if (filename) {
					// use HTML5 a[download] attribute to specify filename
					var a = document.createElement("a");
					// safari doesn't support this yet
					if (typeof a.download === "undefined") {
						window.location.href = downloadUrl;
					} else {
						a.href = downloadUrl;
						a.download = filename;
						document.body.appendChild(a);
						a.click();
					}
				} else {
					window.location.href = downloadUrl;
				}

				// cleanup
				setTimeout(function () {
					document.body.removeChild(a);
					URL.revokeObjectURL(downloadUrl);
				}, 1);
			}
		}
	}
}
