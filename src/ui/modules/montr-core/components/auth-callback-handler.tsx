import * as React from "react";
import { AuthService } from "../services/auth-service";

interface Props {
	children: React.ReactNode;
}

export class AuthCallbackHandler extends React.Component<Props> {

	private _authService: AuthService;

	constructor(props: Props) {
		super(props);

		this._authService = new AuthService();
	}

	public componentDidMount() {
		if (this._authService.isCallback()) {
			this._authService.processCallback();
		}
	}

	render() {
		if (this._authService.isCallback() == false) {
			return this.props.children;
		}

		return null;
	}
}
