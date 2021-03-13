import React from "react";
import { RouteComponentProps } from "react-router";
import i18next from "i18next";
import { DataView, Guid } from "@montr-core/models";
import { Role } from "../models";
import { MetadataService } from "@montr-core/services";
import { DataTabs } from "@montr-core/components";
import { RoleService } from "../services";
import { Locale, Views } from "../module";

interface RouteProps {
    uid?: string;
    tabKey?: string;
}

interface Props extends RouteComponentProps<RouteProps> {
}

interface State {
    loading: boolean;
    data?: Role;
    dataView?: DataView<Role>;
}

export default class PageEditRole extends React.Component<Props, State> {

    private _metadataService = new MetadataService();
    private _roleService = new RoleService();

    constructor(props: Props) {
        super(props);

        this.state = {
            loading: true,
        };
    }

    componentDidMount = async () => {
        await this.fetchData();
    };

    componentWillUnmount = async () => {
        await this._metadataService.abort();
        await this._roleService.abort();
    };

    fetchData = async () => {
        Guid.tryParse(this.props.match.params.uid, async (uid) => {
            // todo: get metadata key from server
            const dataView = await this._metadataService.load(Views.roleEdit);

            const data = await this._roleService.get(uid);

            this.setState({ loading: false, data, dataView });
        });
    };

    render = () => {
        const t = (key: string) => i18next.getFixedT(null, Locale.Namespace)(key),
            { uid, tabKey } = this.props.match.params,
            { loading, data, dataView } = this.state;

        return (<>

            <h1>{uid}</h1>

            <DataTabs
                tabKey={tabKey}
                panes={dataView?.panes}
                // onTabChange={this.handleTabChange}
                disabled={(pane, index) => index > 0 && !data?.uid}
                tabProps={{ data /* , ref: this.createRefForKey(pane.key) */ }}
            />

        </>);
    };
}
