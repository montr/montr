import { Guid } from "@montr-core/models/guid";
import React from "react";

interface Props {
	entityTypeCode: string;
	entityUid: Guid;
}

export default class PaneSettings extends React.Component<Props> {
	render = (): React.ReactNode => {
		const { entityTypeCode, entityUid } = this.props;

		return (
			<>{entityTypeCode} - {entityUid}</>
		);
	};
}
