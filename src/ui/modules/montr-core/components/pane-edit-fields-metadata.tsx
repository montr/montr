import React from "react";
import { Guid } from "../models";
import { PaneEditMetadata } from "./";

interface Props {
    entityTypeCode: string;
    entityUid: Guid;
}

export default class PaneEditFieldsMetadata extends React.Component<Props> {
    render = (): React.ReactNode => {
        const { entityTypeCode, entityUid } = this.props;

        return (
            <PaneEditMetadata
                mode="field"
                entityTypeCode={entityTypeCode}
                entityUid={entityUid}
            />
        );
    };
}
