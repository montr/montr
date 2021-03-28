import React from "react";
import { FormInstance, Spin } from "antd";
import { DataForm } from "@montr-core/components";
import { ApiResult, Guid, IDataField } from "@montr-core/models";
import { MetadataService } from "@montr-core/services";
import { RoleService } from "../services";
import { Role } from "../models";
import { Views } from "../module";

interface Props {
    uid?: Guid;
    data?: Role;
    hideButtons?: boolean;
    formRef?: React.RefObject<FormInstance>;
    onSuccess?: () => void;
    onClose?: () => void;
}

interface State {
    loading: boolean;
    data?: Role;
    fields?: IDataField[];
}

export class FormEditRole extends React.Component<Props, State> {

    private _roleService = new RoleService();
    private _metadataService = new MetadataService();

    constructor(props: Props) {
        super(props);

        this.state = {
            loading: true
        };
    }

    componentDidMount = async () => {
        await this.fetchData();
    };

    componentWillUnmount = async () => {
        await this._roleService.abort();
        await this._metadataService.abort();
    };

    fetchData = async () => {
        const { uid } = this.props;

        const dataView = await this._metadataService.load(Views.roleEdit);

        const data = this.props.data ?? ((uid)
            ? await this._roleService.get(uid)
            : await this._roleService.create()
        );

        this.setState({
            loading: false,
            data: data,
            fields: dataView?.fields || []
        });
    };

    handleSubmit = async (values: Role): Promise<ApiResult> => {
        const { uid, onSuccess } = this.props,
            { data } = this.state;

        const item = {
            uid,
            concurrencyStamp: data.concurrencyStamp,
            ...values
        } as Role;

        const result = (uid)
            ? await this._roleService.update({ item })
            : await this._roleService.insert({ item });

        if (result.success) {

            data.concurrencyStamp = result.concurrencyStamp;

            if (onSuccess) onSuccess();
        }

        return result;
    };

    render = () => {
        const { hideButtons, formRef } = this.props,
            { loading, fields, data } = this.state;

        return (
            <Spin spinning={loading}>

                <DataForm
                    formRef={formRef}
                    hideButtons={hideButtons}
                    fields={fields}
                    data={data}
                    onSubmit={this.handleSubmit} />

            </Spin>
        );
    };
}
