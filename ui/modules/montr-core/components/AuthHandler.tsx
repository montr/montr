import * as React from "react";
import { AuthService } from "../services/";

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
