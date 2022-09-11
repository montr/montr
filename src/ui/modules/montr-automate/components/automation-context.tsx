import React from "react";
import { Automation } from "../models";

export interface AutomationContextProps {
	data: Automation;
	children: React.ReactNode;
}

const defaultState: AutomationContextProps = {
	data: undefined,
	children: undefined
};

export const AutomationContext = React.createContext<AutomationContextProps>(defaultState);

export function withAutomationContext<P extends AutomationContextProps>(Component: React.ComponentType<P>) {
	return (props: Pick<P, Exclude<keyof P, keyof AutomationContextProps>>) => (
		<AutomationContext.Consumer>
			{(ctx) => (
				<Component {...props} {...ctx as P} />
			)}
		</AutomationContext.Consumer>
	);
}

/**
 * @todo: use ClassifierContext or EntityContext instead?
 */
export class AutomationContextProvider extends React.Component<AutomationContextProps> {
	render = (): React.ReactNode => {
		return (
			<AutomationContext.Provider value={this.props}>
				{this.props.children}
			</AutomationContext.Provider>
		);
	};
}
