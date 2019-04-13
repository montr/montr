import * as React from "react";
import { Modal, Spin } from "antd";
import { Guid, IFormField } from "@montr-core/models";
import { withCompanyContext, CompanyContextProps } from "@kompany/components";
import { ClassifierGroupService } from "../services";
import { IClassifierGroup } from "@montr-master-data/models";
import { NotificationService, MetadataService } from "@montr-core/services";
import { DataForm } from "@montr-core/components";

interface IProps extends CompanyContextProps {
	typeCode: string;
	treeCode: string,
	uid?: Guid;
	parentUid?: Guid;
	onCancel?: () => void;
}

interface IState {
	loading: boolean;
	fields?: IFormField[];
	data: IClassifierGroup;
	parent?: IClassifierGroup;
}

class _ModalEditClassifierGroup extends React.Component<IProps, IState> {
	private _notificationService = new NotificationService();
	private _metadataService = new MetadataService();
	private _classifierGroupService = new ClassifierGroupService();

	constructor(props: IProps) {
		super(props);

		this.state = {
			loading: true,
			data: {}
		};
	}

	componentDidMount = async () => {
		await this.fetchData();
	}

	componentWillUnmount = async () => {
		await this._metadataService.abort();
		await this._classifierGroupService.abort();
	}

	fetchData = async () => {
		const { currentCompany, typeCode, treeCode, uid } = this.props;

		if (currentCompany) {

			try {

				const dataView = await this._metadataService.load(`ClassifierGroup/${typeCode}`);

				let data, parent, { parentUid } = this.props;

				if (uid) {
					data = await this._classifierGroupService.get(currentCompany.uid, typeCode, treeCode, uid);
					parentUid = data.parentUid;
				}

				if (parentUid) {
					parent = await this._classifierGroupService.get(currentCompany.uid, typeCode, treeCode, parentUid);
				}

				this.setState({ loading: false, fields: dataView.fields, data: data || {}, parent });

			} catch (error) {
				this._notificationService.error("Ошибка при загрузке данных", error.message);
				this.onCancel();
			}
		}
	}

	onCancel = () => {
		if (this.props.onCancel) this.props.onCancel();
	}

	private save = async (values: IClassifierGroup) => {
	}

	render = () => {
		const { loading, fields, data } = this.state;

		return (
			<Modal visible={!loading} title={data.name} onCancel={this.onCancel}>
				<Spin spinning={this.state.loading}>
					<DataForm fields={fields} data={data} showControls={false} onSave={this.save} />
				</Spin>
			</Modal>
		);
	}
}

export const ModalEditClassifierGroup = withCompanyContext(_ModalEditClassifierGroup);
