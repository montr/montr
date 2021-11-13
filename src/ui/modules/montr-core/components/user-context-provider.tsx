import { User } from "oidc-client";
import * as React from "react";
import { UserContext, UserContextProps } from ".";
import { AuthService } from "../services";

interface UserContextState {
	user?: User;
}

export class UserContextProvider extends React.Component<unknown, UserContextState> {

	private readonly authService = new AuthService();

	constructor(props: unknown) {
		super(props);

		this.state = {
		};
	}

	componentDidMount = () => {
		this.getUser(true);

		this.authService.onAuthenticated((user: User) => {
			this.setState({ user });
		});

		this.authService.addUserSignedOut(() => {
			this.setState({ user: null });
		});
	};

	login = () => {
		this.authService.login()
			.catch(error => { // todo: use logger here and below
				console.error("login error", error);
			});
	};

	logout = () => {
		this.authService.logout()
			.catch(error => {
				console.error("logout error", error);
			});
	};

	getUser = (withLoginSilent: boolean) => {
		this.authService.getUser().then((user: User) => {
			this.setState({ user });
			// todo: move to AuthService (?)
			if (withLoginSilent && user?.expired) {
				this.authService.loginSilent()
					.catch(error => {
						console.error("loginSilent error", error);
					});
			}
		}).catch(error => {
			console.error("getUser error", error);
		});
	};

	render = (): React.ReactNode => {
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
