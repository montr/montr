import { Constants } from "@montr-core/.";
import { ApiResult, Guid } from "@montr-core/models";
import { Fetcher } from "@montr-core/services";

interface ManageUserRoles {
    userUid: Guid;
    roles: string[];
}

export class UserRoleService extends Fetcher {

    addRoles = async (request: ManageUserRoles): Promise<ApiResult> => {
        return this.post(`${Constants.apiURL}/userRole/addRoles`, request);
    };

    removeRoles = async (request: ManageUserRoles): Promise<ApiResult> => {
        return this.post(`${Constants.apiURL}/userRole/removeRoles`, request);
    };

}
