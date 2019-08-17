import * as React from "react";
import { Steps } from "antd";
import { Constants } from "@montr-core/.";
import { Page } from "@montr-core/components";
import { UserContextProps, withUserContext } from "@montr-core/components/"
import { CompanyContextProps, withCompanyContext } from "@kompany/components";

class RegistrationConstants {
	public static UserRegisterUri = Constants.authorityURL + "/Identity/Account/Register";
	public static UserManageUri = Constants.authorityURL + "/Identity/Account/Manage";

	public static PrivateTenderUri = Constants.privateURL;
}

const RegisterUser = (props: UserContextProps) => {
	const { user, login } = props;

	if (user) {
		return (
			<p>
				Пользователь <strong>{user.profile.name} ({user.profile.email})</strong> зарегистрирован.<br />
				Вы можете изменить регистрационные данные в <a href={RegistrationConstants.UserManageUri}>Личном кабинете</a>.
			</p>
		);
	}

	return (
		<p>
			Зарегистрируйте пользователя пройдя по < a href={RegistrationConstants.UserRegisterUri}> ссылке</a>.<br />
			Если вы уже зарегистрированы, войдите в систему пройдя по <a onClick={login}> ссылке</a >.
		</p >
	);
}

const RegisterCompany = (props: UserContextProps & CompanyContextProps) => {
	const { user, currentCompany: company, registerCompany } = props;

	if (user) {

		if (company) {
			return (
				<p>
					Организация <strong>{company.name}</strong> зарегистрирована.<br />
				</p>
			);
		}

		return (
			<p>
				Зарегистрируйте организацию пройдя по <a onClick={registerCompany}>ссылке</a>.
			</p>
		);
	}

	return (
		<p>
			После регистрации пользователя будет доступна регистрация организации.
		</p>
	);
}

const StartWork = (props: UserContextProps & CompanyContextProps) => {
	const { user, currentCompany: company } = props;

	if (user && company) {
		return (
			<p>
				Начните создавать свои торговые процедуры в <a href={RegistrationConstants.PrivateTenderUri}>Личном кабинете</a>.
			</p>
		);
	}

	return (
		<p>
			После регистрации пользователя и организации вы сможете создавать торговые процедуры -
			запросы информации, предложений и цен.
		</p>
	);
}

class _Registration extends React.Component<UserContextProps & CompanyContextProps> {
	render() {
		const { user, currentCompany: company } = this.props;

		let currenStep = 0;

		if (user) {
			currenStep = company ? 2 : 1;
		}

		return (
			<Page title="Регистрация">
				<Steps current={currenStep} direction="vertical">
					<Steps.Step title="Регистрация пользователя" description={<RegisterUser {...this.props} />} />
					<Steps.Step title="Регистрация организации" description={<RegisterCompany {...this.props} />} />
					<Steps.Step title="Начало работы" description={<StartWork {...this.props} />} />
				</Steps>
			</Page>
		);
	}
}

export const Registration = withCompanyContext(withUserContext(_Registration));
