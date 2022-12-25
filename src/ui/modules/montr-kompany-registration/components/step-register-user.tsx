import { UserContextProps, withUserContext } from "@montr-core/components";
import { Patterns } from "@montr-core/module";
import { ProfileModel } from "@montr-idx/models/profile-model";
import { ProfileService } from "@montr-idx/services/profile-service";
import { Locale } from "@montr-kompany-registration/module";
import { Button, Spin } from "antd";
import React from "react";
import { Translation } from "react-i18next";
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
            { loading, profile } = this.state;

        return <Translation ns={Locale.Namespace}>{(t) =>
            <Spin spinning={loading}>

                {!user && <>
                    <p>
                        {t("page-registration.step-register-user.line1")}
                    </p>
                    <p>
                        <Button>
                            <Link to={Patterns.accountRegister}>{t("page-registration.step-register-user.link1")}</Link>
                        </Button>&nbsp;
                        <Button onClick={login}>{t("page-registration.step-register-user.link2")}</Button>
                    </p>
                </>}

                {user && <>
                    <p>
                        {t("page-registration.step-register-user.line2")}
                    </p>
                    <p>
                        {profile && <strong>{profile.displayName} ({profile.userName})</strong>}
                    </p>
                    <p>
                        <Button>
                            <Link to={Patterns.profile}>{t("page-registration.step-register-user.link3")}</Link>
                        </Button>
                    </p>
                </>}

            </Spin>
        }</Translation>;
    };
}

export const StepRegisterUser = withUserContext(WrappedStepRegisterUser);
