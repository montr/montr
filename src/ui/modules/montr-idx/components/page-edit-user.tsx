import React from "react";
import { RouteComponentProps } from "react-router";
import i18next from "i18next";
import { ApiResult, DataView } from "@montr-core/models";
import { User } from "../models";

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
    constructor(props: Props) {
        super(props);

        this.state = {
            loading: true,
        };
    }

    render = () => {
        const t = (key: string) => i18next.getFixedT(null, "tendr")(key),
            { uid, tabKey } = this.props.match.params,
            { loading, data, dataView } = this.state;

        // if (data.uid == null) return null;

        return <h1>{uid}</h1>;
    };
}
