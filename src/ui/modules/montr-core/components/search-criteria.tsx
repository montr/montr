import { IDataField } from "@montr-core/models/data-field";
import React from "react";
import { DataForm } from "./data-form";

interface Props {
    fields: IDataField[];
}

export class SearchCriteria extends React.Component<Props> {
    render(): React.ReactNode {
        const { fields } = this.props;

        return <DataForm fields={fields} />;
    }
}
