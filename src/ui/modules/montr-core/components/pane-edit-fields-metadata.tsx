import React from "react";
import { Guid } from "../models";
import { Views } from "../module";
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
                listViewId={Views.fieldMetadataList}
                formViewId={Views.fieldmetadataForm}
                entityTypeCode={entityTypeCode}
                entityUid={entityUid}
            />
        );
    };
}
