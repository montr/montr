import axios, { AxiosInstance, AxiosResponse, CreateAxiosDefaults } from "axios";
import { Constants } from "../constants";
import { AuthService } from "./auth-service";

// axios.defaults.withCredentials = true;
axios.defaults.baseURL = Constants.apiURL;
axios.defaults.headers.common["X-Requested-With"] = "XMLHttpRequest";
axios.defaults.headers.common["Access-Control-Allow-Origin"] = "*";

export class Fetcher {

	private readonly authService = new AuthService();

	private controller: AbortController;

	private getAxiosInstance(): AxiosInstance {

		this.abort();

		this.controller = new AbortController();

		const config: CreateAxiosDefaults = {
			signal: this.controller.signal,
			withCredentials: true // https://github.com/axios/axios/issues/191#issuecomment-311069164
		};

		const result = axios.create(config);

		result.interceptors.request.use(
			async (config) => {

				const user = await this.authService.getUser();

				if (user && user.access_token) {
					// config.headers.Authorization = `Bearer ${user.access_token}`;
				}

				return config;
			}, (error) => {

				console.log("authenticated.interceptors.request.error", error);

				if (axios.isCancel(error)) {
					return;
				}

				return Promise.reject(error);
			}
		);

		result.interceptors.response.use(null,
			(error) => {

				if (axios.isCancel(error)) {
					return;
				}

				if (error.response && error.response.status === 401) {
					this.authService.login();
					return;
				}

				return Promise.reject(error);
			}
		);

		return result;
	}

	download = async (url: string, body?: unknown): Promise<void> => {
		const axios = this.getAxiosInstance();

		const response = await axios.post(url, body || {}, { responseType: "blob" });

		this.openFile(response);
	};

	post = async (url: string, body?: unknown): Promise<any> => {
		const axios = this.getAxiosInstance();

		const response = await axios.post(url, body || {});

		return response ? response.data : null;
	};

	abort = async (): Promise<void> => {
		if (this.controller) {
			this.controller.abort();
			this.controller = undefined;
		}
	};

	private openFile(response: AxiosResponse<any>) {

		// https://stackoverflow.com/questions/16086162/handle-file-download-from-ajax-post/23797348#23797348
		// see also: https://github.com/kennethjiang/js-file-download
		// see also: https://github.com/eligrey/FileSaver.js/

		if (response.status == 200) {

			let filename = "";
			const disposition = response.headers["content-disposition"];
			if (disposition && disposition.indexOf("attachment") !== -1) {
				const filenameRegex = /filename[^;=\n]*=((['"]).*?\2|[^;\n]*)/;
				const matches = filenameRegex.exec(disposition);
				if (matches != null && matches[1]) filename = matches[1].replace(/['"]/g, '');
			}

			const type = response.headers["content-type"] || "application/octet-stream";

			const blob = typeof File === "function"
				? new File([response.data], filename, { type: type })
				: new Blob([response.data], { type: type });

			// https://developer.mozilla.org/en-US/docs/Web/API/Navigator/msSaveBlob
			/* if (typeof window.navigator.msSaveBlob !== 'undefined') {
				// IE workaround for "HTML7007: One or more blob URLs were revoked by closing the blob for which they were created. These URLs will no longer resolve as the data backing the URL has been freed."
				window.navigator.msSaveBlob(blob, filename);
				return;
			} */

			const URL = window.URL /* || window.webkitURL */;
			const downloadUrl = URL.createObjectURL(blob);

			let a: HTMLAnchorElement;
			if (filename) {
				// use HTML5 a[download] attribute to specify filename
				a = document.createElement("a");
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
