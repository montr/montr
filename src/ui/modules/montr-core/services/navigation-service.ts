export class NavigationService {
	public getUrl(): string {
		return window.location.href;
	}

	public navigate(url: string): void {
		window.location.href = url;
	}

	public getUrlParameter = (name: string) => {
		name = name.replace(/[\[]/, '\\[').replace(/[\]]/, '\\]');
		var regex = new RegExp('[\\?&]' + name + '=([^&#]*)');
		var results = regex.exec(window.location.search);
		return results === null ? '' : decodeURIComponent(results[1].replace(/\+/g, ' '));
	};
}
