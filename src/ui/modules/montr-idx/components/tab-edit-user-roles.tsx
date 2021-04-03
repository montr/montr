import React from "react";
import { Button } from "antd";
import { DataTable, DataTableUpdateToken, Icon, Toolbar } from "@montr-core/components";
import { DataResult } from "@montr-core/models";
import { OperationService } from "@montr-core/services";
import { PaneSelectClassifier } from "@montr-master-data/components";
import { Role, User } from "../models";
import { Api, Views } from "../module";
import { UserRoleService } from "../services/user-role-service";

interface Props {
    data: User;
}

interface State {
    showDrawer?: boolean;
    selectedRowKeys?: string[] | number[];
    updateTableToken: DataTableUpdateToken;
}

export default class TabEditUserRoles extends React.Component<Props, State> {

    private operation = new OperationService();
    private userRoleService = new UserRoleService();

    constructor(props: Props) {
        super(props);

        this.state = {
            updateTableToken: { date: new Date() }
        };
    }

    componentWillUnmount = async (): Promise<void> => {
        await this.userRoleService.abort();
    };

    refreshTable = (resetCurrentPage?: boolean, resetSelectedRows?: boolean): void => {
        const { selectedRowKeys } = this.state;

        this.setState({
            updateTableToken: { date: new Date(), resetCurrentPage, resetSelectedRows },
            selectedRowKeys: resetSelectedRows ? [] : selectedRowKeys
        });
    };

    onLoadTableData = async (loadUrl: string, postParams: any): Promise<DataResult<Role>> => {
        const { data } = this.props;

        const params = { userUid: data?.uid, ...postParams };

        return await this.userRoleService.post(loadUrl, params);
    };

    onSelect = async (keys: string[], rows: Role[]): Promise<void> => {
        const { data } = this.props;

        if (data.uid) {
            const result = await this.operation.execute(async () => {
                return await this.userRoleService.addRoles({
                    userUid: data.uid,
                    roles: rows.map(x => {
                        return x.name;
                    })
                });
            });

            if (result.success) {
                await this.onCloseDrawer();

                await this.refreshTable();
            }
        }
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
                onSelect={this.onSelect}
                onClose={this.onCloseDrawer}
            />}
        </>);
    };
}
