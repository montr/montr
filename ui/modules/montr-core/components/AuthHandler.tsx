import * as React from "react";

import { AuthService } from "../services/AuthService";

interface Props {
}

interface State {
}

export class AuthHandler extends React.Component<Props, State> {

	public authService: AuthService;

	constructor(props: Props) {
		super(props);

		this.authService = new AuthService();
	}

	public componentDidMount() {
		this.authService.signinRedirectCallback().then((user) => {
			window.location.href = "/";
		}).catch(function (e) {
			console.error(e);
		});
	}

	render() {
		return (
			<span />
		);
	}
}
