import * as React from "react";
import { Tag } from "antd";

interface Props {
    statusCode: string;
}

export class StatusTag extends React.Component<Props> {
    render(): React.ReactNode {
        const { statusCode } = this.props;

        const color = statusCode != "draft" ? "green" : undefined;

        return <Tag color={color}>{statusCode}</Tag>;
    }
}
