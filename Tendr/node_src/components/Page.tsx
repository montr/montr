import * as React from "react";

import { PageHeader } from "./";

interface PageProps { title?: string; }

export class Page extends React.Component<PageProps> {
    public render() {
        return (
            <div style={{ padding: "0 50px 16px 50px" }}>

                {this.props.title && <PageHeader>{this.props.title}</PageHeader>}

                {this.props.children}
            </div>
        );
    }
};