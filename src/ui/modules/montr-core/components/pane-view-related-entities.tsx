import { EntityRelationService } from "@montr-core/services";
import { Empty, Spin } from "antd";
import React from "react";
import { EntityRelation, Guid } from "../models";

interface Props {
	entityTypeCode: string;
	entityUid: Guid;
}

interface State {
	loading: boolean;
	data?: EntityRelation[];
}

export default class PaneViewRelatedEntities extends React.Component<Props, State> {

	private readonly entityRelationService = new EntityRelationService();

	constructor(props: Props) {
		super(props);

		this.state = {
			loading: true
		};
	}

	componentDidMount = async () => {
		await this.fetchData();
	};

	fetchData = async (): Promise<void> => {
		const { entityTypeCode, entityUid } = this.props;

		const data = await this.entityRelationService.load({ entityTypeCode, entityUid });

		this.setState({ loading: false, data });
	};

	componentWillUnmount = async () => {
		await this.entityRelationService.abort();
	};

	render = (): React.ReactNode => {
		const { loading, data } = this.state;

		return <Spin spinning={loading}>
			{data?.map(item => {
				return <div>{item.relationType}: {item.relatedEntityTypeCode}@{item.relatedEntityUid}</div>;
			})}

			{data?.length == 0 && <Empty image={Empty.PRESENTED_IMAGE_SIMPLE} />}
		</Spin>;
	};
}
