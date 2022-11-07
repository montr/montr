import { UserContextProps, withUserContext } from "@montr-core/components";
import { Patterns } from "@montr-core/module";
import { CompanyContextProps, withCompanyContext } from "@montr-kompany/components/company-context";
import React from "react";
import { Link } from "react-router-dom";

class WrappedStepRegisterFinish extends React.Component<UserContextProps & CompanyContextProps> {
	render = (): React.ReactNode => {
		const { user, currentCompany: company } = this.props;

		if (user && company) {
			return (
				<p>
					Продолжайте работать в <Link to={Patterns.dashboard}>Личном кабинете</Link>.
				</p>
			);
		}

		return (
			<p>
				После регистрации пользователя и организации вы сможете продолжить работу в Личном кабинете.
			</p>
		);
	};
}

export const StepRegisterFinish = withCompanyContext(withUserContext(WrappedStepRegisterFinish));
