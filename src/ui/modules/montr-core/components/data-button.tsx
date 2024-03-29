import { Button as Btn } from "antd";
import { ButtonType } from "antd/es/button";
import React from "react";
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

                const url = button.action;
                const data = button.props;

                const result = await this.fetcher.post(url, data);

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
