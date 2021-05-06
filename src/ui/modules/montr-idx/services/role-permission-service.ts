import { ApiResult, Guid } from "@montr-core/models";
import { Fetcher } from "@montr-core/services";
import { Api } from "../module";

interface ManageRolePermissions {
    roleUid: Guid;
    addPermissions?: string[];
    removePermissions?: string[];
}

export class RolePermissionService extends Fetcher {

    update = async (request: ManageRolePermissions): Promise<ApiResult> => {
        return this.post(Api.rolePermissionUpdate, request);
    };

}
