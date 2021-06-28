import React from "react";
import { UserContextProps, withUserContext } from "@montr-core/components";
import { IDocument } from "@montr-docs/models";
import { CompanyContextProps, withCompanyContext } from ".";
import { CompanyRegistrationService } from "../services";
import { Button, List, Spin, Tag } from "antd";
import { OperationService } from "@montr-core/services";

interface Props extends UserContextProps, CompanyContextProps {
}

interface State {
    loading: boolean;
    documents: IDocument[];
}

class _StepRegisterCompany extends React.Component<Props, State> {

    private readonly operation = new OperationService();
    private readonly companyRegistrationService = new CompanyRegistrationService();

    constructor(props: Props) {
        super(props);

        this.state = {
            loading: true,
            documents: []
        };
    }

    componentDidMount = async (): Promise<void> => {
        await this.fetchData();
    };

    componentWillUnmount = async (): Promise<void> => {
        await this.companyRegistrationService.abort();
    };

    fetchData = async (): Promise<void> => {

        const documents = await this.companyRegistrationService.listRequests();

        this.setState({ loading: false, documents });
    };

    createRequest = async () => {
        const result = await this.companyRegistrationService.createRequest();
    };

    render = (): React.ReactNode => {
        const { user, currentCompany: company, registerCompany } = this.props,
            { loading, documents } = this.state;

        return (
            <Spin spinning={loading}>

                <List
                    size="small"
                    loading={loading}
                    dataSource={documents}
                    renderItem={item => {

                        return (
                            <List.Item actions={[]}>
                                <List.Item.Meta description={<>{item.documentDate} <Tag>{item.statusCode}</Tag></>} />
                            </List.Item>
                        );
                    }}
                />

                <Button onClick={this.createRequest}>Create company registration request</Button>

            </Spin>
        );

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
