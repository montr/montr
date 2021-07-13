import { DataBreadcrumb } from "@montr-core/components";
import { IMenu } from "@montr-core/models";
import { Classifier } from "@montr-master-data/models";
import React from "react";
import { IDocument } from "../models";
import { Patterns } from "../module";

interface Props {
    type?: Classifier;
    item?: IDocument;
}

export class DocumentBreadcrumb extends React.Component<Props> {
    render = (): React.ReactNode => {

        // todo: localize
        const items: IMenu[] = [
            { name: "Documents", route: Patterns.searchDocuments }
        ];

        return (
            <DataBreadcrumb items={items} />
        );
    };
}
