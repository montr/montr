import { Button as Btn } from "antd";
import { ButtonType } from "antd/lib/button";
import React from "react";
import { Constants } from "..";
import { Button, ConfigurationItemProps } from "../models";
import { Fetcher, OperationService } from "../services";

interface Props extends ConfigurationItemProps {
    button: Button;
}

export class DataButton extends React.Component<Props> {

    private readonly operation = new OperationService();
    private readonly fetcher = new Fetcher();

    onClick = async (): Promise<void> => {
        const { button, onDataChange } = this.props;

        if (button.action) {
            await this.operation.confirm(async () => {
                const url = `${Constants.apiURL}${button.action}`;
                const result = await this.fetcher.post(url, button.props);

                if (result.success) {
                    onDataChange(result);
                }

                return result;
            });
        }
    };

    render = (): React.ReactNode => {
        const { button } = this.props;

        const buttonType = button.type?.toLowerCase() as ButtonType;

        return <Btn type={buttonType} onClick={this.onClick}>{button?.name}</Btn>;
    };
}
