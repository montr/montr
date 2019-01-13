import * as React from "react";
import { Steps } from "antd";
import { Page } from "@montr-core/components";
import { AuthService } from "@montr-core/services";
import { User } from "oidc-client";

class Constants {
	public static UserRegisterUri = "http://idx.montr.io:5050/Identity/Account/Register";
	public static UserLoginUri = "http://idx.montr.io:5050/Identity/Account/Login";
	public static UserManageUri = "http://idx.montr.io:5050/Identity/Account/Manage";

	public static CompanyRegisterUri = "http://kompany.montr.io:5010/Register";
}

interface RegistrationStepProps {
	user: User;
}

class RegisterUser extends React.Component<RegistrationStepProps> {
	render() {
		const { user } = this.props;

		if (user) {
			return (
				<p>
					Пользователь зарегистрирован.<br />
					Вы можете изменить регистрационные данные в <a href={Constants.UserManageUri}>Личном кабинете</a>.
				</p>
			);
		}

		return (
			<p>
				Зарегистрируйте пользователя пройдя по <a href={Constants.UserRegisterUri}>ссылке</a>.<br />
				Если вы уже зарегистрированы, войдите в систему пройдя по <a href={Constants.UserLoginUri}>ссылке</a>.
			</p>
		);
	}
}

class RegisterCompany extends React.Component<RegistrationStepProps> {
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
				После регистрации пользовтеля будет доступна регистрация компании.
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

interface RegistrationProps {
}

interface RegistrationState {
	user?: User;
}

export class Registration extends React.Component<RegistrationProps, RegistrationState> {

	private _authService: AuthService;

	constructor(props: RegistrationProps) {
		super(props);

		this.state = {
		};

		this._authService = new AuthService();
	}

	componentDidMount() {
		this._authService.getUser().then((user: User) => {
			this.setState({ user });
		});

		this._authService.onAuthenticated((user: User) => {
			this.setState({ user });
		})
	}

	render() {

		const { user } = this.state;

		let currenStep = 0;

		if (user) {
			currenStep = 1;
		}

		return (
			<Page title="Регистрация">
				<Steps current={currenStep} direction="vertical">
					<Steps.Step title="Регистрация пользователя" description={<RegisterUser user={user} />} />
					<Steps.Step title="Регистрация компании" description={<RegisterCompany user={user} />} />
					<Steps.Step title="Начало работы" description={<StartWork />} />
				</Steps>
			</Page>
		);
	}
}
