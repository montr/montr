import * as React from "react";
import { User } from "oidc-client";

export interface UserContextProps {
	user?: User,
	login: () => void,
	logout: () => void
}

const defaultState: UserContextProps = {
	login: () => { },
	logout: () => { }
};

export const UserContext = React.createContext<UserContextProps>(defaultState);


/* type Omit<T, K extends keyof T> = Pick<T, Exclude<keyof T, K>>;

export function withUserContext2<
	P extends { userContext?: UserContextProps },
	R = Omit<P, "userContext">
>(
	Component: React.ComponentClass<P> | React.StatelessComponent<P>
): React.FunctionComponent<R> {
	return function BoundComponent(props: R) {
		return (
			<UserContext.Consumer>
				{(userContext) => <Component {...props} userContext={userContext as P} />}
			</UserContext.Consumer>
		);
	};
} */

/* function withUserContext2<P extends IUserContext>(Component: React.ComponentType<P>) {
	return function ThemedComponent(props: Pick<P, Exclude<keyof P, keyof IUserContext>>) {
		return (
			<UserContext.Consumer>
				{(user) => <Component {...props} user={user} />}
			</UserContext.Consumer>
		)
	}
} */

/* export function withUserContext<Props extends UserContextProps>(Child: React.ComponentType<Props>) {
	return (props: Pick<Props, Exclude<keyof Props, keyof UserContextProps>>) => {
		return (
			<UserContext.Consumer>
				{(contextState) => <Child {...props} {...contextState as Props} />}
			</UserContext.Consumer>
		)
	}
} */

/* export function withUserContext<
	P extends { userContext?: UserContextProps },
	R = Pick<P, Exclude<keyof P, keyof UserContextProps>>
>(
	Component: React.ComponentClass<P> | React.StatelessComponent<P> | React.FunctionComponent<P>
): React.FunctionComponent<R> {
	return function BoundComponent(props: R) {
		return (
			<UserContext.Consumer>
				{(ctx) => <Component {...props} {...ctx as P} />}
			</UserContext.Consumer>
		);
	};
} */

/* export function withUserContext<P extends UserContextProps>(Component: React.ComponentType<P>) {
	return function ThemedComponent(props: Pick<P, Exclude<keyof P, keyof UserContextProps>>) {
		return (
			<UserContext.Consumer>
				{(ctx) => <Component {...props} {...ctx as P} />}
			</UserContext.Consumer>
		)
	}
} */

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
