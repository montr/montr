import React from "react";
import { ButtonCancel, ButtonEdit, ButtonSave, PageContextProps, withPageContext } from ".";
import { Button } from "../models";

interface Props extends PageContextProps {
    button: Button;
}

class DataButtonEdit extends React.Component<Props> {

    onEdit = async (): Promise<void> => {
        const { setEditMode } = this.props;

        await setEditMode(true);
    };

    onSubmit = async (): Promise<void> => {
        const { onPageSubmit, setEditMode } = this.props;

        await onPageSubmit();

        await setEditMode(false);
    };

    onCancel = async (): Promise<void> => {
        const { onPageCancel, setEditMode } = this.props;

        await onPageCancel();

        await setEditMode(false);
    };

    render = (): React.ReactNode => {
        const { isEditMode } = this.props;

        if (isEditMode) {
            return <>
                <ButtonSave onClick={this.onSubmit} />
                <ButtonCancel onClick={this.onCancel} />
            </>;
        } else {
            return <ButtonEdit onClick={this.onEdit} />;
        }
    };
}

export default withPageContext(DataButtonEdit);
