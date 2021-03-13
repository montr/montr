import { ApiResult, Guid } from "@montr-core/models";
import { Fetcher } from "@montr-core/services";
import { Api } from "../module";
import { Role } from "../models";

interface ManageRoleRequest {
    item: Role;
}

export class RoleService extends Fetcher {
    create = async (): Promise<Role> => {
        return this.post(Api.roleCreate);
    };

    get = async (uid: Guid): Promise<Role> => {
        return this.post(Api.roleGet, { uid });
    };

    insert = async (request: ManageRoleRequest): Promise<ApiResult> => {
        return this.post(Api.roleInsert, request);
    };

    update = async (request: ManageRoleRequest): Promise<ApiResult> => {
        return this.post(Api.roleUpdate, request);
    };

    delete = async (request: ManageRoleRequest): Promise<ApiResult> => {
        return this.post(Api.roleDelete, request);
    };
}
