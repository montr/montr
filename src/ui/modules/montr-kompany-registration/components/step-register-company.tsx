import { ButtonCreate, ButtonDelete, StatusTag, UserContextProps, withUserContext } from "@montr-core/components";
import { OperationService } from "@montr-core/services";
import { IDocument } from "@montr-docs/models";
import { RouteBuilder } from "@montr-docs/module";
import { Locale } from "@montr-kompany-registration/module";
import { CompanyContextProps, withCompanyContext } from "@montr-kompany/components/company-context";
import { Button, Spin, Typography } from "antd";
import React from "react";
import { Translation } from "react-i18next";
import { Navigate } from "react-router-dom";
import { CompanyRegistrationRequestService } from "../services/company-registration-request-service";

interface Props extends UserContextProps, CompanyContextProps {
}

interface State {
	loading: boolean;
	documents?: IDocument[];
	lastRequest?: IDocument,
	redirectTo?: string;
}

class WrappedStepRegisterCompany extends React.Component<Props, State> {

	private readonly operation = new OperationService();
	private readonly companyRegistrationRequestService = new CompanyRegistrationRequestService();

	constructor(props: Props) {
		super(props);

		this.state = {
			loading: true
		};
	}

	componentDidMount = async (): Promise<void> => {
		await this.fetchData();
	};

	componentWillUnmount = async (): Promise<void> => {
		await this.companyRegistrationRequestService.abort();
	};

	fetchData = async (): Promise<void> => {

		const documents = await this.companyRegistrationRequestService.search(),
			lastRequest = documents.length > 0 ? documents[0] : null;

		this.setState({ loading: false, documents, lastRequest });
	};

	createRequest = async () => {
		const result = await this.operation.execute(() => {
			return this.companyRegistrationRequestService.create();
		});

		if (result.success) {
			this.openRequest({ uid: result.uid });
		}
	};

	openRequest = async (item: IDocument) => {
		this.setState({ redirectTo: RouteBuilder.viewDocument(item.uid, "form") });
	};

	deleteRequest = async (item: IDocument) => {
		await this.operation.confirmDelete(async () => {
			const result = await this.companyRegistrationRequestService.delete(item.uid);

			if (result.success) {
				this.setState({ loading: true });

				await this.fetchData();
			}

			return result;
		});
	};

	render = (): React.ReactNode => {
		const { user, currentCompany: company } = this.props,
			{ redirectTo, loading, lastRequest } = this.state;

		if (redirectTo) {
			return <Navigate to={redirectTo} />;
		}

		return <Translation ns={Locale.Namespace}>{(t) =>
			<Spin spinning={loading}>

				{!user && !company && <>
					<p>{t("page-registration.step-register-company.line1")}</p>
				</>}

				{user && !company && <>
					<p>
						{t("page-registration.step-register-company.line2")}
					</p>

					{!lastRequest && <>
						<p>
							<ButtonCreate onClick={this.createRequest}>{t("page-registration.step-register-company.createRequest")}</ButtonCreate>
						</p>
					</>}

					{lastRequest && <>
						<p>
							<Typography.Text strong>
								{lastRequest.documentNumber} &nbsp;
								{<>{lastRequest.documentDate}</>}
							</Typography.Text>&nbsp;
							<StatusTag statusCode={lastRequest.statusCode} />
						</p>
						<p>
							<Button onClick={() => this.openRequest(lastRequest)}>{t("page-registration.step-register-company.openRequest")}</Button>
							{lastRequest.statusCode == "draft" &&
								<ButtonDelete type="link" onClick={() => this.deleteRequest(lastRequest)}>{t("page-registration.step-register-company.deleteRequest")}</ButtonDelete>}
						</p>
					</>}

				</>}

				{(user && company) && <>
					<p>
						{t("page-registration.step-register-company.line9")}
					</p>
					<p>
						<strong>{company.name}</strong>
					</p>
				</>}

			</Spin>
		}</Translation>;
	};
}

export const StepRegisterCompany = withCompanyContext(withUserContext(WrappedStepRegisterCompany));
