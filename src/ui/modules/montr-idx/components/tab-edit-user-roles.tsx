import React from "react";
import { Button } from "antd";
import { DataTable, DataTableUpdateToken, Icon, Toolbar } from "@montr-core/components";
import { DataResult } from "@montr-core/models";
import { Fetcher } from "@montr-core/services";
import { PaneSelectClassifier } from "@montr-master-data/components";
import { Role, User } from "../models";
import { Api, Views } from "../module";

interface Props {
    data: User;
}

interface State {
    showDrawer?: boolean;
    updateTableToken: DataTableUpdateToken;
}

export default class TabEditUserRoles extends React.Component<Props, State> {

    fetcher = new Fetcher();

    constructor(props: Props) {
        super(props);

        this.state = {
            updateTableToken: { date: new Date() }
        };
    }

    componentWillUnmount = async (): Promise<void> => {
        await this.fetcher.abort();
    };

    onLoadTableData = async (loadUrl: string, postParams: any): Promise<DataResult<Role>> => {
        const { data } = this.props;

        const params = { userUid: data?.uid, ...postParams };

        return await this.fetcher.post(loadUrl, params);
    };

    showAddDrawer = async (): Promise<void> => {
        this.setState({ showDrawer: true });
    };

    onCloseDrawer = async (): Promise<void> => {
        this.setState({ showDrawer: false });
    };

    render = (): React.ReactNode => {

        const { showDrawer, updateTableToken } = this.state;

        return (<>
            <Toolbar clear>
                <Button icon={Icon.Plus} onClick={this.showAddDrawer} type="primary">Добавить</Button>
            </Toolbar>

            <DataTable
                rowKey="uid"
                viewId={Views.userRolesGrid}
                loadUrl={Api.userRoleList}
                onLoadData={this.onLoadTableData}
                updateToken={updateTableToken}
            />

            {showDrawer && <PaneSelectClassifier
                typeCode="role"
                // onSelect={this.onSelect}
                onClose={this.onCloseDrawer}
            />}
        </>);
    };
}
