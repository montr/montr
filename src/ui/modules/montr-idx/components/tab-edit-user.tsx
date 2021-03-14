import React from "react";
import { User } from "../models";
import { FormEditUser } from ".";

interface Props {
    data: User;
}

export default class TabEditUser extends React.Component<Props> {

    render = (): React.ReactNode => {

        const { data } = this.props;

        return (<>
            {data && <FormEditUser
                uid={data?.uid}
                data={data}
            />}
        </>);
    };
}
