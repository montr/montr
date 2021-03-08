import React from "react";
import { User } from "../models";
import { FormEditUser } from ".";

interface Props {
    data: User;
}

interface State {
}

export default class TabEditUser extends React.Component<Props, State> {

    render = () => {

        const { data } = this.props;

        return (<>
            {data && <FormEditUser
                uid={data?.uid}
                data={data}
            />}
        </>);
    };
}
