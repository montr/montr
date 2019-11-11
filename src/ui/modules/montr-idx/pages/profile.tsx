import React from "react";
import { IFormField, IApiResult } from "@montr-core/models";
import { MetadataService } from "@montr-core/services";
import { Views } from "../module";
import { IProfileModel } from "../models";
import { ProfileService } from "../services/";
import { Translation } from "react-i18next";
import { Page, DataForm, Toolbar, DataBreadcrumb, PageHeader } from "@montr-core/components";
import { Spin, Button } from "antd";

interface IProps {
}

interface IState {
	loading: boolean;
	data: IProfileModel;
	fields?: IFormField[];
}

export default class Register extends React.Component<IProps, IState> {

	private _metadataService = new MetadataService();
	private _profileService = new ProfileService();

	constructor(props: IProps) {
		super(props);

		this.state = {
			data: {},
			loading: true
		};
	}

	componentDidMount = async () => {
		await this.fetchData();
	}

	componentWillUnmount = async () => {
		await this._metadataService.abort();
		await this._profileService.abort();
	}

	fetchData = async () => {
		const data = await this._profileService.get();

		const dataView = await this._metadataService.load(Views.formProfile);

		this.setState({ loading: false, data, fields: dataView.fields });
	}

	save = async (values: IProfileModel): Promise<IApiResult> => {
		return await this._profileService.update(values);
	}

	handleChangePassword = () => {
		/* const { t } = this.props;

		Modal.confirm({
			title: t("confirm.title"),
			content: t("cancel.confirm.content"),
			onOk: () => {
				this._eventService
					.cancel(this.props.match.params.uid)
					.then((result: IApiResult) => {
						message.success(t("operation.success"));
						this.fetchData();
					});
			}
		}); */
	}

	render = () => {
		const { loading, fields, data } = this.state;

		return (
			<Translation ns="idx">
				{(t) => <Page
				title={<>
					<Toolbar float="right">
					<Button onClick={this.handleChangePassword}>{t("button.changePassword")}</Button>
					</Toolbar>
	
					<DataBreadcrumb items={[]} />
					<PageHeader>{t("page.profile.title")}</PageHeader>
				</>}>
					<h3>{t("page.profile.subtitle")}</h3>

					<Spin spinning={loading}>
						<DataForm
							fields={fields}
							data={data}
							onSubmit={this.save}
							successMessage="Your profile has been updated"
						/>
					</Spin>
				</Page>}
			</Translation>
		);
	}
}
