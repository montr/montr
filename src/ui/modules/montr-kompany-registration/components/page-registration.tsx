import { Page } from "@montr-core/components";
import { UserContextProps, withUserContext } from "@montr-core/components/";
import { CompanyContextProps, withCompanyContext } from "@montr-kompany/components";
import { Steps } from "antd";
import * as React from "react";
import { StepRegisterCompany, StepRegisterFinish, StepRegisterUser } from ".";

class _PageRegistration extends React.Component<UserContextProps & CompanyContextProps> {
	render() {
		const { user, currentCompany: company } = this.props;

		let currenStep = 0;

		if (user) {
			currenStep = company ? 2 : 1;
		}

		return (
			<Page title="Регистрация">
				<Steps current={currenStep} direction="vertical">
					<Steps.Step title="Регистрация пользователя" description={<StepRegisterUser />} />
					<Steps.Step title="Регистрация организации" description={<StepRegisterCompany />} />
					<Steps.Step title="Начало работы" description={<StepRegisterFinish />} />
				</Steps>
			</Page>
		);
	}
}

export default withCompanyContext(withUserContext(_PageRegistration));
