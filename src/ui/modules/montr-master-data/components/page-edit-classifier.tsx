import { DataTabs, Page, PageHeader } from "@montr-core/components";
import { DataView } from "@montr-core/models";
import { Spin } from "antd";
import * as React from "react";
import { RouteComponentProps, useNavigate, useParams } from "react-router";
import { ClassifierBreadcrumb } from ".";
import { Classifier, ClassifierType } from "../models";
import { EntityTypeCode, RouteBuilder, Views } from "../module";
import { ClassifierMetadataService, ClassifierService, ClassifierTypeService } from "../services";

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

	private readonly classifierMetadataService = new ClassifierMetadataService();
	private readonly classifierTypeService = new ClassifierTypeService();
	private readonly classifierService = new ClassifierService();

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
		await this.classifierMetadataService.abort();
		await this.classifierTypeService.abort();
		await this.classifierService.abort();
	};

	fetchData = async (): Promise<void> => {
		const { typeCode, uid, parentUid } = useParams();

		const dataView = await this.classifierMetadataService.view(typeCode, Views.classifierTabs);

		const type = await this.classifierTypeService.get({ typeCode });

		const data = (uid)
			? await this.classifierService.get(typeCode, uid)
			: await this.classifierService.create(typeCode, parentUid);

		this.setState({ loading: false, dataView, type, data });
	};

	handleDataChange = (data: Classifier): void => {
		const { typeCode, uid } = useParams();

		if (uid) {
			this.setState({ data });
		}
		else {
			const navigate = useNavigate();

			const path = RouteBuilder.editClassifier(typeCode, data.uid);

			navigate(path);
		}
	};

	handleTabChange = (tabKey: string): void => {
		const { typeCode, uid } = useParams();

		const navigate = useNavigate();

		const path = RouteBuilder.editClassifier(typeCode, uid, tabKey);

		navigate(path);
	};

	render = (): React.ReactNode => {
		const { tabKey } = useParams(),
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
						paneProps={{
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
