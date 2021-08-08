import { ApiResult } from ".";

export interface ConfigurationItemProps extends Record<string, unknown> {
    onDataChange: (result: ApiResult) => void;
}
