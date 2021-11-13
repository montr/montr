import { Button as Btn } from "antd";
import { ButtonType } from "antd/lib/button";
import React from "react";
import { ButtonCancel, ButtonSave, PageContextProps, withPageContext } from ".";
import { Button } from "../models";

interface Props extends PageContextProps {
    button: Button;
}

class ButtonEdit extends React.Component<Props> {

    onClick = async (): Promise<void> => {
        const { setEditMode } = this.props;
    };

    render = (): React.ReactNode => {
        const { button, isEditMode, setEditMode } = this.props;

        if (isEditMode) {
            return <>
                <ButtonSave onClick={() => setEditMode(false)} />
                <ButtonCancel onClick={() => setEditMode(false)} />
            </>;
        } else {
            const buttonType = button?.type?.toLowerCase() as ButtonType;
            return <Btn type={buttonType} onClick={() => setEditMode(true)}>{button?.name}</Btn>;
        }
    };
}

const ButtonEditWrapper = withPageContext(ButtonEdit);

export default ButtonEditWrapper;
