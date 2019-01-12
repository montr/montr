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
		if (this._authService.isOidcCallback()) {
			this._authService.processOidcCallback();
		}
	}

	render() {
		if (this._authService.isOidcCallback() == false) {
			return this.props.children;
		}

		return null;
	}
}
