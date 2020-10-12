import React from "react";
import { Modal, Spin } from "antd";
import { DataForm } from ".";
import { EntityStatusService, MetadataService } from "../services";
import { Guid, IApiResult, IDataField, EntityStatus } from "../models";
import { FormInstance } from "antd/lib/form";

interface IProps {
    entityTypeCode: string;
    entityUid: Guid;
    uid?: Guid;
    onSuccess?: (data: EntityStatus) => void;
    onCancel?: () => void;
}

interface IState {
    loading: boolean;
    fields?: IDataField[];
    data: Partial<EntityStatus>;
}

export class ModalEditEntityStatus extends React.Component<IProps, IState> {

    private _metadataService = new MetadataService();
    private _entityStatusService = new EntityStatusService();

    private _formRef = React.createRef<FormInstance>();

    constructor(props: IProps) {
        super(props);

        this.state = {
            loading: true,
            data: {}
        };
    }

    componentDidMount = async () => {
        await this.fetchData();
    };

    componentWillUnmount = async () => {
        await this._metadataService.abort();
        await this._entityStatusService.abort();
    };

    fetchData = async () => {
        const { uid } = this.props;

        const dataView = await this._metadataService.load(`EntityStatus/Form`);

        const fields = dataView.fields;

        let data;

        if (uid) {
            // data = await this._classifierTreeService.get(typeCode, uid);
        }
        else {
            // todo: load defaults from server
            data = {};
        }

        this.setState({ loading: false, fields, data });
    };

    onOk = async (e: React.MouseEvent<any>) => {
        await this._formRef.current.submit();
    };

    onCancel = () => {
        if (this.props.onCancel) this.props.onCancel();
    };

    save = async (values: EntityStatus): Promise<IApiResult> => {
        const { entityTypeCode, entityUid, uid, onSuccess } = this.props;

        let data: EntityStatus,
            result: IApiResult;

        if (uid) {
            // data = { uid: uid, ...values };

            // result = await this._entityStatusService.update(typeCode, data);
        }
        else {
            const insertResult = await this._entityStatusService.insert({
                entityTypeCode, entityUid, item: values
            });

            // todo: reload from server?
            // data = { uid: insertResult.uid, ...values };

            result = insertResult;
        }

        if (result.success) {
            if (onSuccess) await onSuccess(data);
        }

        return result;
    };

    render = () => {
        const { loading, fields, data } = this.state;

        return (
            <Modal visible={!loading} title={data.name}
                onOk={this.onOk} onCancel={this.onCancel}
                okText="Сохранить" width="640px">
                <Spin spinning={loading}>
                    <DataForm
                        formRef={this._formRef}
                        fields={fields}
                        data={data}
                        showControls={false}
                        onSubmit={this.save}
                    />
                </Spin>
            </Modal>
        );
    };
}