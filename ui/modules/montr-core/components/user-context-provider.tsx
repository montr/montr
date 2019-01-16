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
			// this.getUser(false);
		})
	}

	login = () => {
		this._authService.login()
			.catch(error => { // todo: use logger here and below
				console.log("login error", error);
			});
	};

	logout = () => {
		this._authService.logout()
			.catch(error => {
				console.log("logout error", error);
			});
	};

	getUser = (withLoginSilent: boolean) => {
		this._authService.getUser().then((user: User) => {
			this.setState({ user });
			// todo: move to AuthService (?)
			if (withLoginSilent && (!user || user.expired)) {
				this._authService.loginSilent()
					.catch(error => {
						console.log("loginSilent error", error);
					});
			}
		}).catch(error => {
			console.log("getUser error", error);
		});
	};

	render() {
		const { user } = this.state;

		const userContext: UserContextProps = {
			user: user,
			login: this.login,
			logout: this.logout
		};

		return (
			<UserContext.Provider value={userContext}>
				{this.props.children}
			</UserContext.Provider>
		);
	}
}
