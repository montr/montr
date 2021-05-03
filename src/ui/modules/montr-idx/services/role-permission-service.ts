import { ApiResult, Guid } from "@montr-core/models";
import { Fetcher } from "@montr-core/services";
import { Api } from "../module";

interface ManageRolePermissions {
    roleUid: Guid;
    permissions: string[];
}

export class RolePermissionService extends Fetcher {

    add = async (request: ManageRolePermissions): Promise<ApiResult> => {
        return this.post(Api.rolePermissionAdd, request);
    };

    remove = async (request: ManageRolePermissions): Promise<ApiResult> => {
        return this.post(Api.rolePermissionRemove, request);
    };

}
