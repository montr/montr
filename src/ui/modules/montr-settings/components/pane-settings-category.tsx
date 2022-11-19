import { withParams } from "@montr-core/components/react-router-wrappers";
import { Guid } from "@montr-core/models/guid";
import React from "react";
import PaneSettings from "./pane-settings";

interface RouteProps {
    category?: string;
}

interface Props {
    params: RouteProps;
}

class PaneSettingsCategory extends React.Component<Props> {

    getRouteProps = (): RouteProps => {
        return this.props.params;
    };

    render = (): React.ReactNode => {
        const { category } = this.getRouteProps();

        return <PaneSettings
            entityTypeCode="application"
            entityUid={Guid.empty}
            category={category}
        />;
    };
}

export default withParams(PaneSettingsCategory);
