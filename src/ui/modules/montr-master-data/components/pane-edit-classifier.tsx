import { ButtonCancel, ButtonSave, Toolbar } from "@montr-core/components";
import { Guid } from "@montr-core/models/guid";
import { Drawer, FormInstance } from "antd";
import React from "react";
import { ClassifierType } from "../models/classifier-type";
import { FormEditClassifier } from "./form-edit-classifier";

interface Props {
    type: ClassifierType;
    uid?: Guid;
    parentUid?: Guid;
    onSuccess?: () => void;
    onClose?: () => void;
}

export class PaneEditClassifier extends React.Component<Props> {

    formRef = React.createRef<FormInstance>();

    handleSubmitClick = async (): Promise<void> => {
        await this.formRef.current.submit();
    };

    render = (): React.ReactNode => {
        const { type, uid, parentUid, onSuccess, onClose } = this.props;

        return (
            <Drawer
                title="Classifier"
                closable={true}
                onClose={onClose}
                open={true}
                width={720}
                footer={
                    <Toolbar clear size="small" float="right">
                        <ButtonCancel onClick={onClose} />
                        <ButtonSave onClick={this.handleSubmitClick} />
                    </Toolbar>}>

                <FormEditClassifier
                    type={type}
                    uid={uid}
                    parentUid={parentUid}
                    formRef={this.formRef}
                    hideButtons={true}
                    onSuccess={onSuccess}
                />

            </Drawer>
        );
    };
}
