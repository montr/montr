import { Button as Btn } from "antd";
import React from "react";
import { Constants } from "..";
import { Button } from "../models";
import { Fetcher, OperationService } from "../services";

interface Props {
    button: Button;
}

export class DataButton extends React.Component<Props> {

    private readonly operation = new OperationService();
    private readonly fetcher = new Fetcher();

    onClick = async (): Promise<void> => {
        const { button } = this.props;

        await this.operation.confirm(async () => {
            const url = `${Constants.apiURL}${button.action}`;
            const result = await this.fetcher.post(url);

            if (result.success) {
                console.log(button);
            }

            return result;
        });
    };

    render = (): React.ReactNode => {
        const { button } = this.props;

        return <Btn type="primary" onClick={this.onClick}>{button?.name}</Btn>;
    };
}
