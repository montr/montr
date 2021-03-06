import * as React from "react";
import { Alert, Spin } from "antd";
import { Translation } from "react-i18next";
import { ApiResult, AppState, IDataField, IIndexer } from "../models";
import { MetadataService, SetupService } from "../services";
import { DataForm, Page } from ".";
import { Views } from "../module";
import { Constants } from "..";

interface State {
    loading: boolean;
    fields?: IDataField[];
}

export default class PageSetup extends React.Component<null, State> {

    private _metadataService = new MetadataService();
    private _setupService = new SetupService();

    constructor() {
        super(null);

        this.state = {
            loading: true
        };
    }

    componentDidMount = async (): Promise<void> => {
        await this.fetchData();
    };

    componentWillUnmount = async (): Promise<void> => {
        await this._metadataService.abort();
        await this._setupService.abort();
    };

    fetchData = async (): Promise<void> => {
        const dataView = await this._metadataService.load(Views.setupForm);

        this.setState({ loading: false, fields: dataView.fields });
    };

    save = async (values: IIndexer): Promise<ApiResult> => {
        return await this._setupService.save(values);
    };

    render = (): React.ReactNode => {
        const { loading, fields } = this.state,
            appState = Constants.appState,
            data = {};

        if (appState == AppState.Initialized) {
            return (
                <Translation>
                    {(t) => <Page title={t("page.setup.title")}>

                        <Alert type="warning" message={t("page.setup.initializedMessage")} />

                    </Page>}
                </Translation>
            );
        }

        return (
            <Translation>
                {(t) => <Page title={t("page.setup.title")}>

                    <p>{t("page.setup.subtitle")}</p>

                    <Spin spinning={loading}>
                        <DataForm
                            layout="vertical"
                            fields={fields}
                            data={data}
                            onSubmit={this.save}
                        />
                    </Spin>

                </Page>}
            </Translation>
        );
    };
}
