import * as React from "react";
import { User } from "oidc-client";

export interface UserContextProps {
	user?: User,

	// todo: remove methods below
	login: () => void,
	logout: () => void;
}

const defaultState: UserContextProps = {
	login: () => { },
	logout: () => { }
};

export const UserContext = React.createContext<UserContextProps>(defaultState);

// https://hackernoon.com/state-management-with-react-context-typescript-and-graphql-fb6264314a15
// https://github.com/lilybarrett/jawn-with-graphql-and-react-context

export function withUserContext<P extends UserContextProps>(Component: React.ComponentType<P>) {
	return (props: Pick<P, Exclude<keyof P, keyof UserContextProps>>) => (
		<UserContext.Consumer>
			{(ctx) => (
				<Component {...props} {...ctx as P} />
			)}
		</UserContext.Consumer>
	);
}
