import * as React from "react";

export class PageHeader extends React.Component {
    public render() {
        return (
            <h1>{this.props.children}</h1>
        );
    }
};