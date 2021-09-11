import { UserContextProps, withUserContext } from "@montr-core/components";
import { Patterns } from "@montr-core/module";
import React from "react";
import { Link } from "react-router-dom";

class _StepRegisterUser extends React.Component<UserContextProps>{
    render = (): React.ReactNode => {
        const { user, login } = this.props;

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
}

export const StepRegisterUser = withUserContext(_StepRegisterUser);
