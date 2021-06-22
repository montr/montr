import * as React from "react";
import { DataTabs, Page, PageHeader } from "@montr-core/components";
import { DataView } from "@montr-core/models";
import { RouteComponentProps } from "react-router";
import { Spin } from "antd";
import { ClassifierService, ClassifierTypeService, ClassifierMetadataService } from "../services";
import { Classifier, ClassifierType } from "../models";
import { ClassifierBreadcrumb } from ".";
import { EntityTypeCode, RouteBuilder, Views } from "../module";

interface RouteProps {
	typeCode: string;
	uid?: string;
	parentUid?: string;
	tabKey?: string;
}

interface Props extends RouteComponentProps<RouteProps> {
}

interface State {
	loading: boolean;
	dataView?: DataView<Classifier>;
	type?: ClassifierType;
	data?: Classifier;
}

export default class PageEditClassifier extends React.Component<Props, State> {

	private _classifierMetadataService = new ClassifierMetadataService();
	private _classifierTypeService = new ClassifierTypeService();
	private _classifierService = new ClassifierService();

	constructor(props: Props) {
		super(props);

		this.state = {
			loading: true
		};
	}

	componentDidMount = async (): Promise<void> => {
		await this.fetchData();
	};

	componentDidUpdate = async (prevProps: Props): Promise<void> => {
		if (this.props.match.params.typeCode !== prevProps.match.params.typeCode ||
			this.props.match.params.uid !== prevProps.match.params.uid) {
			await this.fetchData();
		}
	};

	componentWillUnmount = async (): Promise<void> => {
		await this._classifierMetadataService.abort();
		await this._classifierTypeService.abort();
		await this._classifierService.abort();
	};

	fetchData = async (): Promise<void> => {
		const { typeCode, uid, parentUid } = this.props.match.params;

		const dataView = await this._classifierMetadataService.load(typeCode, Views.classifierTabs);

		const type = await this._classifierTypeService.get({ typeCode });

		const data = (uid)
			? await this._classifierService.get(typeCode, uid)
			: await this._classifierService.create(typeCode, parentUid);

		this.setState({ loading: false, dataView, type, data });
	};

	handleDataChange = (data: Classifier): void => {
		const { typeCode, uid } = this.props.match.params;

		if (uid) {
			this.setState({ data });
		}
		else {
			const path = RouteBuilder.editClassifier(typeCode, data.uid);

			this.props.history.push(path);
		}
	};

	handleTabChange = (tabKey: string): void => {
		const { typeCode, uid } = this.props.match.params;

		const path = RouteBuilder.editClassifier(typeCode, uid, tabKey);

		this.props.history.replace(path);
	};

	render = (): React.ReactNode => {
		const { tabKey } = this.props.match.params,
			{ loading, dataView, type, data } = this.state;

		return (
			<Page title={<>
				<ClassifierBreadcrumb type={type} item={data} />
				<PageHeader>{data?.name}</PageHeader>
			</>}>
				<Spin spinning={loading}>

					<DataTabs
						tabKey={tabKey}
						panes={dataView?.panes}
						onTabChange={this.handleTabChange}
						disabled={(_, index) => index > 0 && !data?.uid}
						tabProps={{
							type,
							data,
							onDataChange: this.handleDataChange,
							entityTypeCode: EntityTypeCode.classifier,
							entityUid: data?.uid
						}}
					/>

				</Spin>
			</Page>
		);
	};
}
