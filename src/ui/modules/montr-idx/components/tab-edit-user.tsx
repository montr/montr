import React from "react";
import { User } from "../models";
import { PaneEditUser } from ".";

interface Props {
    data: User;
}

interface State {
}

export default class TabEditUser extends React.Component<Props, State> {

    render = () => {
        return (
            <PaneEditUser
                uid={this.props.data?.uid}
            // onSuccess={this.handleSuccess}
            // onClose={this.closePane}
            />
        );
    };
}
