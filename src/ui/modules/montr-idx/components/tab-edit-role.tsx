import React from "react";
import { Role } from "../models";
import { FormEditRole } from ".";

interface Props {
    data: Role;
}

export default class TabEditRole extends React.Component<Props> {

    render = (): React.ReactNode => {

        const { data } = this.props;

        return (<>
            {data && <FormEditRole
                uid={data?.uid}
                data={data}
            />}
        </>);
    };
}
