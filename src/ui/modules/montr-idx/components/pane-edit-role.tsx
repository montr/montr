import React from "react";
import { Drawer, FormInstance } from "antd";
import { Guid } from "@montr-core/models";
import { ButtonCancel, ButtonSave, Toolbar } from "@montr-core/components";
import { FormEditRole } from ".";

interface Props {
    uid?: Guid;
    onSuccess?: () => void;
    onClose?: () => void;
}

// todo: create PaneEdit to remove PaneEditRole and PaneEditUser. see also ModalChangeEmail
export class PaneEditRole extends React.Component<Props> {

    formRef = React.createRef<FormInstance>();

    handleSubmitClick = async (e: React.MouseEvent<any>): Promise<void> => {
        await this.formRef.current.submit();
    };

    render = (): React.ReactNode => {
        const { uid, onSuccess, onClose } = this.props;

        return (
            <Drawer
                title="Role"
                closable={true}
                onClose={onClose}
                visible={true}
                width={720}
                footer={
                    <Toolbar clear size="small" float="right">
                        <ButtonCancel onClick={onClose} />
                        <ButtonSave onClick={this.handleSubmitClick} />
                    </Toolbar>}>

                <FormEditRole
                    uid={uid}
                    formRef={this.formRef}
                    hideButtons={true}
                    onSuccess={onSuccess}
                />

            </Drawer>
        );
    };
}
