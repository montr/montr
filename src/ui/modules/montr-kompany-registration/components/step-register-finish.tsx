import { UserContextProps, withUserContext } from "@montr-core/components";
import { Patterns } from "@montr-core/module";
import { Locale } from "@montr-kompany-registration/module";
import { CompanyContextProps, withCompanyContext } from "@montr-kompany/components/company-context";
import { Button } from "antd";
import React from "react";
import { Translation } from "react-i18next";
import { Link } from "react-router-dom";

class WrappedStepRegisterFinish extends React.Component<UserContextProps & CompanyContextProps> {
	render = (): React.ReactNode => {
		const { user, currentCompany: company } = this.props;

		return <Translation ns={Locale.Namespace}>{(t) =>
			<>
				{(!user || !company) && <>
					<p>
						{t("page-registration.step-finish.line1")}
					</p>
				</>}

				{(user && company) && <>
					<p>
						{t("page-registration.step-finish.line2")}
					</p>
					<p>
						<Button>
							<Link to={Patterns.dashboard}>{t("page-registration.step-finish.link1")}</Link>
						</Button>
					</p>
				</>}

			</>
		}</Translation>;
	};
}

export const StepRegisterFinish = withCompanyContext(withUserContext(WrappedStepRegisterFinish));
