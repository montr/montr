import React from "react";
import { Link } from "react-router-dom";
import { Patterns } from "@montr-core/module";
import { UserContextProps, withUserContext } from "@montr-core/components";
import { CompanyContextProps, withCompanyContext } from ".";

class _StepRegisterFinish extends React.Component<UserContextProps & CompanyContextProps> {
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

export const StepRegisterFinish = withCompanyContext(withUserContext(_StepRegisterFinish));
