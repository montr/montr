import { UserContextProps, withUserContext } from "@montr-core/components";
import { Patterns } from "@montr-core/module";
import { ProfileModel } from "@montr-idx/models/profile-model";
import { ProfileService } from "@montr-idx/services/profile-service";
import React from "react";
import { Link } from "react-router-dom";

interface State {
    loading: boolean;
    profile?: ProfileModel;
}

class WrappedStepRegisterUser extends React.Component<UserContextProps, State>{

    private readonly profileService = new ProfileService();

    constructor(props: UserContextProps) {
        super(props);

        this.state = {
            loading: true
        };
    }

    componentDidMount = async (): Promise<void> => {
        await this.fetchData();
    };

    componentWillUnmount = async (): Promise<void> => {
        await this.profileService.abort();
    };

    componentDidUpdate = async (prevProps: UserContextProps): Promise<void> => {
        if (this.props.user !== prevProps.user) {
            await this.fetchData();
        }
    };

    fetchData = async (): Promise<void> => {
        const { user } = this.props;

        const profile = user ? await this.profileService.get() : null;

        this.setState({ loading: false, profile });
    };

    render = (): React.ReactNode => {
        const { user, login } = this.props,
            { profile } = this.state;

        if (user) {
            return (
                <p>
                    Пользователь <strong>{profile?.displayName} ({profile?.userName})</strong> зарегистрирован.<br />
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

export const StepRegisterUser = withUserContext(WrappedStepRegisterUser);
