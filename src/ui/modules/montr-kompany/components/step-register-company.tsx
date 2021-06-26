import React from "react";
import { UserContextProps, withUserContext } from "@montr-core/components";
import { CompanyContextProps, withCompanyContext } from ".";

class _StepRegisterCompany extends React.Component<UserContextProps & CompanyContextProps> {
    render = (): React.ReactNode => {
        const { user, currentCompany: company, registerCompany } = this.props;

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
}

export const StepRegisterCompany = withCompanyContext(withUserContext(_StepRegisterCompany));
