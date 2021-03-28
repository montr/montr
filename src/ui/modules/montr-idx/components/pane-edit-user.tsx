import React from "react";
import { Drawer, FormInstance } from "antd";
import { Guid } from "@montr-core/models";
import { ButtonCancel, ButtonSave, Toolbar } from "@montr-core/components";
import { FormEditUser } from ".";

interface Props {
    uid?: Guid;
    onSuccess?: () => void;
    onClose?: () => void;
}

interface State {
}

export class PaneEditUser extends React.Component<Props, State> {

    private _formRef = React.createRef<FormInstance>();

    handleSubmitClick = async (e: React.MouseEvent<any>) => {
        await this._formRef.current.submit();
    };

    render = () => {
        const { uid, onSuccess, onClose } = this.props;

        return (
            <Drawer
                title="Пользователь"
                closable={true}
                onClose={onClose}
                visible={true}
                width={720}
                footer={
                    <Toolbar clear size="small" float="right">
                        <ButtonCancel onClick={onClose} />
                        <ButtonSave onClick={this.handleSubmitClick} />
                    </Toolbar>}>

                <FormEditUser
                    uid={uid}
                    formRef={this._formRef}
                    hideButtons={true}
                    onSuccess={onSuccess}
                />

            </Drawer>
        );
    };
}
