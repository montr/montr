import { Page, UserContextProps, withUserContext } from "@montr-core/components/";
import { Locale } from "@montr-kompany-registration/module";
import { CompanyContextProps, withCompanyContext } from "@montr-kompany/components/company-context";
import { Steps } from "antd";
import * as React from "react";
import { Translation } from "react-i18next";
import { StepRegisterCompany } from "./step-register-company";
import { StepRegisterFinish } from "./step-register-finish";
import { StepRegisterUser } from "./step-register-user";

class WrappedPageRegistration extends React.Component<UserContextProps & CompanyContextProps> {
	render() {
		const { user, currentCompany: company } = this.props;

		let currenStep = 0;

		if (user) {
			currenStep = company ? 2 : 1;
		}

		return <Translation ns={Locale.Namespace}>{(t) =>
			<Page title={t("page-registration.title")}>
				<Steps current={currenStep} direction="vertical" items={[
					{ title: t("page-registration.step-register-user.title"), description: <StepRegisterUser /> },
					{ title: t("page-registration.step-register-company.title"), description: <StepRegisterCompany /> },
					{ title: t("page-registration.step-finish.title"), description: <StepRegisterFinish /> },
				]} />
			</Page>
		}</Translation>;
	}
}

export default withCompanyContext(withUserContext(WrappedPageRegistration));
