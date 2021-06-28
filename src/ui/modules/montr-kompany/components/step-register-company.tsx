import React from "react";
import { Redirect } from "react-router";
import { UserContextProps, withUserContext } from "@montr-core/components";
import { IDocument } from "@montr-docs/models";
import { CompanyContextProps, withCompanyContext } from ".";
import { CompanyRegistrationRequestService } from "../services";
import { Button, List, Spin, Tag } from "antd";
import { OperationService } from "@montr-core/services";
import { RouteBuilder } from "@montr-docs/module";

interface Props extends UserContextProps, CompanyContextProps {
}

interface State {
    loading: boolean;
    documents: IDocument[];
    redirectTo?: string;
}

class _StepRegisterCompany extends React.Component<Props, State> {

    private readonly operation = new OperationService();
    private readonly companyRegistrationRequestService = new CompanyRegistrationRequestService();

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
        await this.companyRegistrationRequestService.abort();
    };

    fetchData = async (): Promise<void> => {

        const documents = await this.companyRegistrationRequestService.search();

        this.setState({ loading: false, documents });
    };

    createRequest = async () => {
        const result = await this.operation.execute(() => {
            return this.companyRegistrationRequestService.create();
        });

        if (result.success) {
            this.setState({ redirectTo: RouteBuilder.editDocument(result.uid) });
        }
    };

    openRequest = async (item: IDocument) => {
        const redirectTo = item.statusCode == "draft"
            ? RouteBuilder.editDocument(item.uid)
            : RouteBuilder.viewDocument(item.uid);

        this.setState({ redirectTo });
    };

    deleteRequest = async (item: IDocument) => {
        const result = await this.operation.confirmDelete(() => {
            return this.companyRegistrationRequestService.delete(item.uid);
        });

        if (result.success) {
            this.setState({ loading: true });

            await this.fetchData();
        }
    };

    render = (): React.ReactNode => {
        const { user, currentCompany: company, registerCompany } = this.props,
            { redirectTo, loading, documents } = this.state;

        if (redirectTo) {
            return <Redirect to={redirectTo} push={true} />;
        }

        return (
            <Spin spinning={loading}>

                <List
                    size="small"
                    dataSource={documents}
                    renderItem={item => {

                        const actions = [];

                        if (item.statusCode == "draft") {
                            actions.push(<a onClick={() => this.openRequest(item)}>Edit</a>);
                            actions.push(<a onClick={() => this.deleteRequest(item)}>Delete</a>);
                        } else {
                            actions.push(<a onClick={() => this.openRequest(item)}>View</a>);
                        }

                        const tagColor = item.statusCode != "draft" ? "green" : undefined;

                        return (
                            <List.Item actions={actions}>
                                <List.Item.Meta
                                    title={'Company Registration Request'}
                                    description={<>
                                        Document Date: {item.documentDate}
                                        <Tag color={tagColor}>{item.statusCode}</Tag>
                                    </>} />
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
