import { DateHelper } from "@montr-core/services/date-helper";
import { Descriptions } from "antd";
import React from "react";
import { IDocument } from "../models";

interface Props {
	document: IDocument;
}

export class DocumentSignificantInfo extends React.Component<Props> {
	render = (): React.ReactNode => {
		const { document } = this.props;

		return <Descriptions size="small" column={1}>
			<Descriptions.Item label="Number">{document.documentNumber}</Descriptions.Item>
			<Descriptions.Item label="Date">{DateHelper.toLocaleDateTimeString(document.documentDate)}</Descriptions.Item>
		</Descriptions>;
	};
}
