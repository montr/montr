import * as React from "react";
import { Steps } from "antd";
import { Page } from "@montr-core/components";
import { UserContextProps, withUserContext } from "@montr-core/components/"
import { User } from "oidc-client";

class Constants {
	public static UserRegisterUri = "http://idx.montr.io:5050/Identity/Account/Register";
	public static UserLoginUri = "http://idx.montr.io:5050/Identity/Account/Login";
	public static UserManageUri = "http://idx.montr.io:5050/Identity/Account/Manage";

	public static CompanyRegisterUri = "http://kompany.montr.io:5010/Register";
}

class RegisterUser extends React.Component<UserContextProps> {
	render() {
		const { user, login } = this.props;

		if (user) {
			return (
				<p>
					Пользователь <strong>{user.profile.name} ({user.profile.email})</strong> зарегистрирован.<br />
					Вы можете изменить регистрационные данные в <a href={Constants.UserManageUri}>Личном кабинете</a>.
				</p>
			);
		}

		return (
			<p>
				Зарегистрируйте пользователя пройдя по <a href={Constants.UserRegisterUri}>ссылке</a>.<br />
				Если вы уже зарегистрированы, войдите в систему пройдя по <a onClick={login}>ссылке</a>.
			</p>
		);
	}
}

class RegisterCompany extends React.Component<UserContextProps> {
	render() {
		const { user } = this.props;

		if (user) {
			return (
				<p>
					Зарегистрируйте компанию  пройдя по <a href={Constants.CompanyRegisterUri}>ссылке</a>.
				</p>
			);
		}

		return (
			<p>
				После регистрации пользователя будет доступна регистрация компании.
			</p>
		);
	}
}

class StartWork extends React.Component {
	render() {
		return (
			<p>Начните создавать свои события.</p>
		);
	}
}

/* const Test: React.FunctionComponent<UserContextProps> = ({ user }) => {
	if (user)
		return (<p>12345 {user.scope}</p>);
	return (<p>NOP</p>)
};

const TestWithUser = withUserContext(Test); */

interface RegistrationProps {
}

interface RegistrationState {
	user?: User;
}

class _Registration extends React.Component<RegistrationProps & UserContextProps, RegistrationState> {
	render() {

		const { user } = this.props;

		let currenStep = 0;

		if (user) {
			currenStep = 1;
		}

		return (
			<Page title="Регистрация">
				<Steps current={currenStep} direction="vertical">
					<Steps.Step title="Регистрация пользователя" description={<RegisterUser {...this.props} />} />
					<Steps.Step title="Регистрация компании" description={<RegisterCompany {...this.props} />} />
					<Steps.Step title="Начало работы" description={<StartWork />} />
					{/* <Steps.Step title="TEst" description={<TestWithUser />} /> */}
				</Steps>
			</Page>
		);
	}
}

export const Registration = withUserContext(_Registration);
