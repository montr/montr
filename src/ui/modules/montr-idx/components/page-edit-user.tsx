import React from "react";
import { RouteComponentProps } from "react-router";
import i18next from "i18next";
import { ApiResult, DataView } from "@montr-core/models";
import { User } from "../models";
import { MetadataService } from "@montr-core/services";
import { DataTabs } from "@montr-core/components";

interface RouteProps {
    uid?: string;
    tabKey?: string;
}

interface Props extends RouteComponentProps<RouteProps> {
}

interface State {
    loading: boolean;
    data?: User;
    dataView?: DataView<User>;
}

export default class PageEditUser extends React.Component<Props, State> {

    private _metadataService = new MetadataService();

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
    };

    fetchData = async () => {
        // todo: get metadata key from server
        const dataView = await this._metadataService.load("User/Edit");

        this.setState({ loading: false, dataView });
    };

    render = () => {
        const t = (key: string) => i18next.getFixedT(null, "tendr")(key),
            { uid, tabKey } = this.props.match.params,
            { loading, data, dataView } = this.state;

        // if (data.uid == null) return null;

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
