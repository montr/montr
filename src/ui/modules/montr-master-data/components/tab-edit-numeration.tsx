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

    private readonly classifierMetadataService = new ClassifierMetadataService();
    private readonly numeratorEntityService = new NumeratorEntityService();

    constructor(props: Props) {
        super(props);

        this.state = {
            loading: true
        };
    }

    componentDidMount = async (): Promise<void> => {
        await this.fetchData();
    };

    componentWillUnmount = async (): Promise<void> => {
        await this.classifierMetadataService.abort();
        await this.numeratorEntityService.abort();
    };

    fetchData = async (): Promise<void> => {

        const { entityTypeCode, entityUid } = this.props;

        // todo: load metadata from common service
        const dataView = await this.classifierMetadataService.view(null, Views.numeratorEntityForm);

        const data: NumeratorEntity = await this.numeratorEntityService.get(entityTypeCode, entityUid);

        this.setState({ loading: false, fields: dataView.fields, data });
    };

    save = async (values: NumeratorEntity): Promise<ApiResult> => {

        const { entityTypeCode, entityUid } = this.props;

        const updated = {
            entityTypeCode: entityTypeCode,
            entityUid: entityUid,
            ...values
        };

        const result = await this.numeratorEntityService.save(updated);

        return result;
    };

    render = (): React.ReactNode => {
        const { loading, fields, data } = this.state;

        return (
            <Spin spinning={loading}>
                <DataForm fields={fields} data={data} onSubmit={this.save} />
            </Spin>
        );
    };
}
