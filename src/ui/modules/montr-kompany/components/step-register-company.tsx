import React from "react";
import { Redirect } from "react-router";
import { ButtonCreate, ButtonDelete, UserContextProps, withUserContext } from "@montr-core/components";
import { IDocument } from "@montr-docs/models";
import { CompanyContextProps, withCompanyContext } from ".";
import { CompanyRegistrationRequestService } from "../services";
import { Button, List, Space, Spin, Tag, Typography } from "antd";
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
        await this.operation.confirmDelete(async () => {
            const result = await this.companyRegistrationRequestService.delete(item.uid);

            if (result.success) {
                this.setState({ loading: true });

                await this.fetchData();
            }

            return result;
        });
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
                            actions.push(<ButtonDelete type="link" onClick={() => this.deleteRequest(item)} />);
                            actions.push(<Button onClick={() => this.openRequest(item)}>Edit</Button>);
                        } else {
                            actions.push(<Button onClick={() => this.openRequest(item)}>View</Button>);
                        }

                        return (
                            <List.Item actions={actions}>
                                <List.Item.Meta
                                    description={
                                        <Space>
                                            Number: <Typography.Text>{item.documentNumber || "n/a"}</Typography.Text>
                                            Date: <Typography.Text>{item.documentDate}</Typography.Text>

                                            <Tag color={item.statusCode != "draft" ? "green" : undefined}>{item.statusCode}</Tag>
                                        </Space>}
                                />
                            </List.Item>
                        );
                    }}
                />

                <ButtonCreate onClick={this.createRequest}>Create company registration request</ButtonCreate>

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