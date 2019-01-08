import * as React from "react";

import { AuthService } from "../services/AuthService";

interface Props {
}

interface State {
}

export class AuthHandler extends React.Component<Props, State> {

	private _authService: AuthService;

	constructor(props: Props) {
		super(props);

		this._authService = new AuthService();
	}

	public componentDidMount() {
		this._authService.signinRedirectCallback().then((user) => {
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
