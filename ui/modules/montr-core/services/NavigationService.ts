export class NavigationService {
	public getUrl(): string {
		return window.location.href;
	}

	public navigate(url: string): void {
		window.location.href = url;
	}
}
