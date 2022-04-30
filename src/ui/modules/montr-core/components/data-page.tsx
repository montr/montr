import React from "react";
import { PageContextProvider } from ".";

interface Props {
	children: React.ReactNode;
}

export class DataPage extends React.Component<Props> {
	render = (): React.ReactNode => {
		return (
			<PageContextProvider>
				{this.props.children}
			</PageContextProvider>
		);
	};
}
