import React from "react";
import { Drawer, FormInstance, Spin } from "antd";
import { ApiResult, Guid, IDataField } from "@montr-core/models";
import { ButtonCancel, ButtonSave, DataForm, Toolbar } from "@montr-core/components";
import { MetadataService } from "@montr-core/services";
import { User } from "../models";
import { UserService } from "../services";
import { Views } from "../module";

interface Props {
    uid?: Guid;
    showControls?: boolean;
    onSuccess?: () => void;
    onClose?: () => void;
}

interface State {
    loading: boolean;
    data?: User;
    fields?: IDataField[];
}

export class PaneEditUser extends React.Component<Props, State> {

    private _userService = new UserService();
    private _metadataService = new MetadataService();
    private _formRef = React.createRef<FormInstance>();

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

        const data = (uid)
            ? await this._userService.get(uid)
            : await this._userService.create();

        const dataView = await this._metadataService.load(Views.userEdit);

        this.setState({
            loading: false,
            data: data as User,
            fields: dataView?.fields || []
        });
    };

    handleSubmitClick = async (e: React.MouseEvent<any>) => {
        await this._formRef.current.submit();
    };

    handleSubmit = async (values: User): Promise<ApiResult> => {
        const { uid, onSuccess } = this.props,
            { data } = this.state;

        const item = {
            uid,
            concurrencyStamp: data.concurrencyStamp,
            ...values
        } as User;

        let result;

        if (uid) {
            result = await this._userService.update({ item });
        }
        else {
            result = await this._userService.insert({ item });
        }

        if (result.success && onSuccess) {
            onSuccess();
        }

        return result;
    };

    render = () => {
        const { showControls, onClose } = this.props,
            { loading, fields, data } = this.state;

        return (<>
            <Spin spinning={loading}>
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

                    <DataForm
                        formRef={this._formRef}
                        showControls={showControls}
                        fields={fields}
                        data={data}
                        onSubmit={this.handleSubmit} />

                </Drawer>
            </Spin>
        </>);
    };
}
