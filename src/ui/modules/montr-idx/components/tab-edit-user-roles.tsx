import React from "react";
import { DataTable } from "@montr-core/components";
import { DataResult } from "@montr-core/models";
import { Fetcher } from "@montr-core/services";
import { Role, User } from "../models";
import { Api, Views } from "../module";

interface Props {
    data: User;
}

export default class TabEditUserRoles extends React.Component<Props> {

    fetcher = new Fetcher();

    componentWillUnmount = async (): Promise<void> => {
        await this.fetcher.abort();
    };

    onLoadTableData = async (loadUrl: string, postParams: any): Promise<DataResult<Role>> => {
        const { data } = this.props;

        const params = { userUid: data?.uid, ...postParams };

        return await this.fetcher.post(loadUrl, params);
    };

    render = (): React.ReactNode => {
        return (<>
            <DataTable
                rowKey="uid"
                viewId={Views.userRolesGrid}
                loadUrl={Api.userRoleList}
                onLoadData={this.onLoadTableData}
            />
        </>);
    };
}
