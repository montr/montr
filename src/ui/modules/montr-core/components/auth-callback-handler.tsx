import * as React from "react";
import { AuthService } from "../services/auth-service";

interface Props {
	children: React.ReactNode;
}

export class AuthCallbackHandler extends React.Component<Props> {

	private readonly authService: AuthService = new AuthService();

	constructor(props: Props) {
		super(props);
	}

	public componentDidMount() {
		if (this.authService.isCallback()) {
			this.authService.processCallback();
		}
	}

	render() {
		if (this.authService.isCallback() == false) {
			return this.props.children;
		}

		return null;
	}
}
