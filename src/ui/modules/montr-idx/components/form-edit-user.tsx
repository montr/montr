import React from "react";
import { FormInstance, Spin } from "antd";
import { DataForm } from "@montr-core/components";
import { ApiResult, Guid, IDataField } from "@montr-core/models";
import { MetadataService } from "@montr-core/services";
import { UserService } from "../services";
import { User } from "../models";
import { Views } from "../module";

interface Props {
    uid?: Guid;
    data?: User;
    showControls?: boolean;
    formRef?: React.RefObject<FormInstance>;
    onSuccess?: () => void;
    onClose?: () => void;
}

interface State {
    loading: boolean;
    data?: User;
    fields?: IDataField[];
}

export class FormEditUser extends React.Component<Props, State> {

    private _userService = new UserService();
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
        await this._userService.abort();
        await this._metadataService.abort();
    };

    fetchData = async () => {
        const { uid } = this.props;

        const dataView = await this._metadataService.load(Views.userEdit);

        const data = this.props.data ?? ((uid)
            ? await this._userService.get(uid)
            : await this._userService.create()
        );

        this.setState({
            loading: false,
            data: data,
            fields: dataView?.fields || []
        });
    };

    handleSubmit = async (values: User): Promise<ApiResult> => {
        const { uid, onSuccess } = this.props,
            { data } = this.state;

        const item = {
            uid,
            concurrencyStamp: data.concurrencyStamp,
            ...values
        } as User;

        const result = (uid)
            ? await this._userService.update({ item })
            : await this._userService.insert({ item });

        if (result.success) {

            data.concurrencyStamp = result.concurrencyStamp;

            if (onSuccess) onSuccess();
        }

        return result;
    };

    render = () => {
        const { showControls, formRef } = this.props,
            { loading, fields, data } = this.state;

        return (
            <Spin spinning={loading}>

                <DataForm
                    formRef={formRef}
                    showControls={showControls}
                    fields={fields}
                    data={data}
                    onSubmit={this.handleSubmit} />

            </Spin>
        );
    };
}
