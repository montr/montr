import React from "react";
import { FormInstance, Spin } from "antd";
import { ApiResult, Guid, IDataField } from "@montr-core/models";
import { DataForm } from "@montr-core/components";
import { Classifier, ClassifierType } from "../models";
import { ClassifierLinkService, ClassifierMetadataService, ClassifierService } from "../services";
import { Views } from "../module";

interface Props {
    type: ClassifierType;
    uid?: Guid;
    parentUid?: Guid;
    data?: Classifier;
    hideButtons?: boolean;
    formRef?: React.RefObject<FormInstance>;
    onSuccess?: (data: Classifier) => void;
}

interface State {
    loading: boolean;
    data?: Classifier;
    fields?: IDataField[];
}

export class FormEditClassifier extends React.Component<Props, State> {

    private readonly classifierMetadataService = new ClassifierMetadataService();
    private readonly classifierService = new ClassifierService();
    private readonly classifierLinkService = new ClassifierLinkService();

    constructor(props: Props) {
        super(props);

        this.state = {
            loading: true
        };
    }

    componentDidMount = async (): Promise<void> => {
        await this.fetchData();
    };

    componentDidUpdate = async (prevProps: Props): Promise<void> => {
        if (this.props.type !== prevProps.type) {
            await this.fetchData();
        }
    };

    componentWillUnmount = async (): Promise<void> => {
        await this.classifierMetadataService.abort();
        await this.classifierService.abort();
    };

    fetchData = async (): Promise<void> => {
        const { type, uid, parentUid } = this.props;

        if (type) {

            const dataView = await this.classifierMetadataService.view(type.code, Views.classifierForm);

            /* const fields = dataView.fields;

            const parentUidField = fields.find(x => x.key == "parentUid") as IClassifierField;

            if (parentUidField) {
                parentUidField.typeCode = type.code;
                // parentUidField.treeUid = treeUid;
            } */

            const data = this.props.data ?? ((uid)
                ? await this.classifierService.get(type.code, uid)
                : await this.classifierService.create(type.code, parentUid)
            );

            // ParentUid of classifier in default hierarchy is not loaded with classifier info,
            // so we need to load it wuth separate request.
            // todo: edit parent of classifier with separate operation (separate form),
            // only classifier fields should be displayed in classifier edit form (without ParentUid field) 
            if (uid && type.hierarchyType == "Groups") {

                const links = await this.classifierLinkService.list({ typeCode: type.code, itemUid: uid });

                const defaultLink = links.rows.find(x => x.tree.code == "default");

                if (defaultLink) data.parentUid = defaultLink.group.uid;
            }

            this.setState({ loading: false, data, fields: dataView.fields });
        }
    };

    submit = async (values: Classifier): Promise<ApiResult> => {
        const { type, uid, onSuccess } = this.props,
            { data } = this.state;

        const item = {
            uid,
            concurrencyStamp: data?.concurrencyStamp,
            ...values
        } as Classifier;

        const result = (uid)
            ? await this.classifierService.update(type.code, item)
            : await this.classifierService.insert(type.code, item);

        if (result.success) {

            data.concurrencyStamp = result.concurrencyStamp;

            if (onSuccess) onSuccess(data);
        }

        return result;
    };

    render = (): React.ReactNode => {
        const { hideButtons, formRef } = this.props,
            { data, fields, loading } = this.state;

        return (
            <Spin spinning={loading}>
                <DataForm
                    formRef={formRef}
                    hideButtons={hideButtons}
                    onSubmit={this.submit}
                    fields={fields}
                    data={data}
                />
            </Spin>
        );
    };
}
