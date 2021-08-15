import React from "react";
import { Guid } from "../models";
import { PaneEditMetadata } from "./";

interface Props {
    entityTypeCode: string;
    entityUid: Guid;
}

export default class PaneEditFormMetadata extends React.Component<Props> {
    render = (): React.ReactNode => {
        const { entityTypeCode, entityUid } = this.props;

        return (
            <PaneEditMetadata
                mode="form"
                entityTypeCode={entityTypeCode}
                entityUid={entityUid}
            />
        );
    };
}
