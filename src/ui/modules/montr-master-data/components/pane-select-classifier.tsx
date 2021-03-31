import { Drawer } from "antd";
import React from "react";
import { PaneSearchClassifier } from ".";

interface Props {
    typeCode: string;
    onSelect?: (keys: string[] | number[]) => Promise<void>;
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
