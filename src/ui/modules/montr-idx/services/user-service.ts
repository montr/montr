import { ApiResult, Guid } from "@montr-core/models";
import { Fetcher } from "@montr-core/services";
import { Api } from "../module";
import { User } from "../models";

interface ManageUserRequest {
    item: User;
}

export class UserService extends Fetcher {
    create = async (): Promise<User> => {
        return this.post(Api.userCreate);
    };

    get = async (uid: Guid): Promise<User> => {
        return this.post(Api.userGet, { uid });
    };

    insert = async (request: ManageUserRequest): Promise<ApiResult> => {
        return this.post(Api.userInsert, request);
    };

    update = async (request: ManageUserRequest): Promise<ApiResult> => {
        return this.post(Api.userUpdate, request);
    };

    delete = async (request: ManageUserRequest): Promise<ApiResult> => {
        return this.post(Api.userDelete, request);
    };
}
