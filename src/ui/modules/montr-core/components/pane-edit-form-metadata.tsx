import React from "react";
import { Guid } from "../models";
import { Views } from "../module";
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
                listViewId={Views.formMetadataList}
                formViewId={Views.formMetadataForm}
                entityTypeCode={entityTypeCode}
                entityUid={entityUid}
            />
        );
    };
}
