import { Guid } from "@montr-core/models";
import React from "react";

export interface AutomationContextProps {
	entityTypeCode: string;
	entityUid: Guid | string;
	children: React.ReactNode;
}

const defaultState: AutomationContextProps = {
	entityTypeCode: undefined,
	entityUid: undefined,
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

export class AutomationContextProvider extends React.Component<AutomationContextProps> {
	render = (): React.ReactNode => {
		return (
			<AutomationContext.Provider value={this.props}>
				{this.props.children}
			</AutomationContext.Provider>
		);
	};
}
