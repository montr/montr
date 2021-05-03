import { ApiResult, Guid } from "@montr-core/models";
import { Fetcher } from "@montr-core/services";
import { Api } from "../module";

interface ManageUserRoles {
    userUid: Guid;
    roles: string[];
}

export class UserRoleService extends Fetcher {

    addRoles = async (request: ManageUserRoles): Promise<ApiResult> => {
        return this.post(Api.userRoleAddRoles, request);
    };

    removeRoles = async (request: ManageUserRoles): Promise<ApiResult> => {
        return this.post(Api.userRoleRemoveRoles, request);
    };

}
