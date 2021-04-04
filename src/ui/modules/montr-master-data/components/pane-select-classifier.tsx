import React from "react";
import { Drawer } from "antd";
import { Classifier } from "../models";
import { PaneSearchClassifier } from ".";

interface Props {
    typeCode: string;
    onSelect?: (keys: string[] | number[], rows: Classifier[]) => Promise<void>;
    onClose?: () => void;
}

export class PaneSelectClassifier extends React.Component<Props> {

    render = (): React.ReactNode => {
        const { typeCode, onSelect, onClose } = this.props;

        return (
            <Drawer
                // title="Контрагенты" // todo: load from api
                closable={false}
                onClose={onClose}
                visible={true}
                width={1024}>
                <PaneSearchClassifier
                    mode="drawer"
                    typeCode={typeCode}
                    onSelect={onSelect}
                />
            </Drawer>
        );
    };
}
