import React from "react";
import { Spin } from "antd";
import { DataForm } from "@montr-core/components";
import { ApiResult, Guid, IDataField } from "@montr-core/models";
import { NumeratorEntity } from "../models";
import { ClassifierMetadataService, NumeratorEntityService } from "../services";
import { Views } from "../module";

interface Props {
    entityTypeCode: string;
    entityUid: Guid;
}

interface State {
    loading: boolean;
    fields?: IDataField[];
    data?: NumeratorEntity;
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

        const { entityTypeCode, entityUid } = this.props;

        // todo: load metadata from common service
        const dataView = await this._classifierMetadataService.load(null, Views.numeratorEntityForm);

        const data: NumeratorEntity = await this._numeratorEntityService.get(entityTypeCode, entityUid);

        this.setState({ loading: false, fields: dataView.fields, data });
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
        const { loading, fields, data } = this.state;

        return (
            <Spin spinning={loading}>
                <DataForm fields={fields} data={data} onSubmit={this.save} />
            </Spin>
        );
    }
}
