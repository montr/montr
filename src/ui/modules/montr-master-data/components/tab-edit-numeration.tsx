import React from "react";
import { ApiResult, Guid, IDataField } from "@montr-core/models";
import { NumeratorEntity } from "../models";
import { ClassifierMetadataService, NumeratorEntityService } from "../services";
import { Views } from "@montr-master-data/module";
import { Spin } from "antd";
import { DataForm } from "@montr-core/components";

interface Props {
    entityTypeCode: string;
    entityUid: Guid;
    data: NumeratorEntity;
}

interface State {
    loading: boolean;
    fields?: IDataField[];
}

export default class TabEditNumeration extends React.Component<Props, State> {

    private _classifierMetadataService = new ClassifierMetadataService();
    private _numeratorEntityService = new NumeratorEntityService();

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
        await this._classifierMetadataService.abort();
        await this._numeratorEntityService.abort();
    };

    fetchData = async () => {

        const dataView = await this._classifierMetadataService.load(null, Views.numeratorEntityForm);

        this.setState({ loading: false, fields: dataView.fields });
    };

    save = async (values: NumeratorEntity): Promise<ApiResult> => {
        const { entityTypeCode, entityUid } = this.props;

        const updated = {
            entityTypeCode: entityTypeCode,
            entityUid: entityUid,
            ...values
        };

        const result = await this._numeratorEntityService.save(updated);

        return result;
    };

    render() {
        const { data } = this.props,
            { loading, fields } = this.state;

        return (
            <Spin spinning={loading}>
                <DataForm fields={fields} data={data} onSubmit={this.save} />
            </Spin>
        );
    }
}
