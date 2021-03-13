import React from "react";
import { Role } from "../models";
import { FormEditRole } from ".";

interface Props {
    data: Role;
}

interface State {
}

export default class TabEditRole extends React.Component<Props, State> {

    render = () => {

        const { data } = this.props;

        return (<>
            {data && <FormEditRole
                uid={data?.uid}
                data={data}
            />}
        </>);
    };
}
