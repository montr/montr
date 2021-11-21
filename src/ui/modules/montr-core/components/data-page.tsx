import React from "react";
import { PageContextProvider } from ".";

export class DataPage extends React.Component {
    render = (): React.ReactNode => {
        return (
            <PageContextProvider>
                {this.props.children}
            </PageContextProvider>
        );
    };
}
