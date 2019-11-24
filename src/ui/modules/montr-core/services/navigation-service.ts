import { Constants } from "../constants";

export class NavigationService {
	public navigate(url: string): void {
		window.location.href = url;
	}

	public getUrl(): string {
		return window.location.href;
	}

	public getPathname(): string {
		return window.location.pathname;
	}

	public getReturnUrlParameter = () => {
		return this.getUrlParameter(Constants.returnUrlParam);
	};

	public getUrlParameter = (name: string) => {
		/* name = name.replace(/[\[]/, '\\[').replace(/[\]]/, '\\]');
		var regex = new RegExp('[\\?&]' + name + '=([^&#]*)');
		var results = regex.exec(window.location.search);
		return results === null ? '' : decodeURIComponent(results[1].replace(/\+/g, ' ')); */

		return new URLSearchParams(window.location.search).get(name);
	};
}
