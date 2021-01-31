import * as React from "react";
import { User } from "oidc-client";
import { AuthService } from "../services";
import { UserContext, UserContextProps } from ".";

interface UserContextState {
	user?: User;
}

export class UserContextProvider extends React.Component<any, UserContextState> {

	_authService = new AuthService();

	constructor(props: any) {
		super(props);

		this.state = {
		};
	}

	componentDidMount = () => {
		this.getUser(true);

		this._authService.onAuthenticated((user: User) => {
			this.setState({ user });
		});

		this._authService.addUserSignedOut(() => {
			this.setState({ user: null });
		});
	};

	login = () => {
		this._authService.login()
			.catch(error => { // todo: use logger here and below
				console.error("login error", error);
			});
	};

	logout = () => {
		this._authService.logout()
			.catch(error => {
				console.error("logout error", error);
			});
	};

	getUser = (withLoginSilent: boolean) => {
		this._authService.getUser().then((user: User) => {
			this.setState({ user });
			// todo: move to AuthService (?)
			if (withLoginSilent && user?.expired) {
				this._authService.loginSilent()
					.catch(error => {
						console.error("loginSilent error", error);
					});
			}
		}).catch(error => {
			console.error("getUser error", error);
		});
	};

	render = () => {
		const { user } = this.state;

		const context: UserContextProps = {
			user: user,
			login: this.login,
			logout: this.logout
		};

		return (
			<UserContext.Provider value={context}>
				{this.props.children}
			</UserContext.Provider>
		);
	};
}
