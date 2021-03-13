import React from "react";
import { Translation } from "react-i18next";
import { Page, Toolbar, DataBreadcrumb, PageHeader, DataTable, ButtonAdd, DataTableUpdateToken } from "@montr-core/components";
import { Guid } from "@montr-core/models";
import { Api, Locale, Views } from "../module";
import { Role } from "../models";
import { PaneEditRole } from ".";

interface Props {
}

interface State {
    showPane?: boolean;
    editUid?: Guid;
    selectedRowKeys?: string[] | number[];
    updateTableToken?: DataTableUpdateToken;
}

export default class PageSearcRoles extends React.Component<Props, State> {

    constructor(props: Props) {
        super(props);

        this.state = {
        };
    }

    refreshTable = async (resetCurrentPage?: boolean, resetSelectedRows?: boolean) => {
        const { selectedRowKeys } = this.state;

        this.setState({
            updateTableToken: { date: new Date(), resetCurrentPage, resetSelectedRows },
            selectedRowKeys: resetSelectedRows ? [] : selectedRowKeys
        });
    };

    showAddPane = () => {
        this.setState({ showPane: true, editUid: null });
    };

    showEditPane = (data: Role) => {
        this.setState({ showPane: true, editUid: data?.uid });
    };

    closePane = () => {
        this.setState({ showPane: false });
    };

    handleSuccess = () => {
        this.setState({ showPane: false });
        this.refreshTable(false);
    };

    render = () => {
        const { showPane, editUid, selectedRowKeys, updateTableToken } = this.state;

        return (
            <Translation ns={Locale.Namespace}>
                {(t) => <>
                    <Page title={<>
                        <Toolbar float="right">
                            <ButtonAdd type="primary" onClick={this.showAddPane} />
                        </Toolbar>

                        <DataBreadcrumb items={[{ name: "Roles" }]} />
                        <PageHeader>Roles</PageHeader>
                    </>}>

                        <DataTable
                            rowKey="uid"
                            rowActions={[{ name: t("button.edit"), onClick: this.showEditPane }]}
                            viewId={Views.gridSearchRoles}
                            loadUrl={Api.roleList}
                            updateToken={updateTableToken}
                        />

                        {showPane &&
                            <PaneEditRole
                                uid={editUid}
                                onSuccess={this.handleSuccess}
                                onClose={this.closePane}
                            />
                        }

                    </Page>
                </>}
            </Translation>
        );
    };
};
