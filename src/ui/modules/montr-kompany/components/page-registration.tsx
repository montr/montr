import * as React from "react";
import { Steps } from "antd";
import { Patterns } from "@montr-core/module";
import { Page } from "@montr-core/components";
import { UserContextProps, withUserContext } from "@montr-core/components/";
import { CompanyContextProps, withCompanyContext } from "@montr-kompany/components";
import { Link } from "react-router-dom";

const RegisterUser = (props: UserContextProps) => {
	const { user, login } = props;

	if (user) {
		return (
			<p>
				Пользователь <strong>{user.profile.name} ({user.profile.email})</strong> зарегистрирован.<br />
				Вы можете изменить регистрационные данные в <Link to={Patterns.profile}>Личном кабинете</Link>.
			</p>
		);
	}

	return (
		<p>
			Зарегистрируйте пользователя пройдя по <Link to={Patterns.accountRegister}> ссылке</Link>.<br />
			Если вы уже зарегистрированы, войдите в систему пройдя по <a onClick={login}> ссылке</a >.
		</p>
	);
};

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
};

const StartWork = (props: UserContextProps & CompanyContextProps) => {
	const { user, currentCompany: company } = props;

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

const Registration = withCompanyContext(withUserContext(_Registration));

export default Registration;
