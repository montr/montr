import { Tag } from "antd";
import * as React from "react";

interface Props {
    statusCode: string;
}

export class StatusTag extends React.Component<Props> {
    render(): React.ReactNode {
        const { statusCode } = this.props;

        let color;

        if (statusCode == "published") color = "green";
        if (statusCode == "system") color = "red";

        return <Tag color={color}>{statusCode}</Tag>;
    }
}
