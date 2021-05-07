import i18next from "i18next";
import { ApiResult, ValidationProblemDetails, ApiResultError } from "../models";
import { NotificationService, NavigationService } from ".";

interface OperationExecuteOptions {
	successMessage?: string;
	errorMessage?: string;
	showFieldErrors?: (result: ApiResult) => void;
	showConfirm?: boolean;
	confirmTitle?: string;
	confirmContent?: string;
}

export class OperationService {

	private readonly navigation = new NavigationService();
	private readonly notification = new NotificationService();

	execute = async (operation: () => Promise<ApiResult>, options?: OperationExecuteOptions): Promise<ApiResult> => {

		const t = (key: string) => i18next.t(key);

		const showFieldErrors = (result: ApiResult) => {
			// todo: show detailed field errors as notification.error?
			if (options?.showFieldErrors) {
				options.showFieldErrors(result);
			}
			else if (result?.errors) {
				result.errors.forEach(x => {
					x.messages.forEach(e => {
						this.notification.error(e);
					});
				});
			}
		};

		const executeInternal = async (): Promise<ApiResult> => {

			const message = this.notification.loading(t("operation.executing"));

			try {
				const result = await operation();

				if (result && result.success) {
					this.notification.success(result.message ?? options?.successMessage ?? t("operation.success"));
				}
				else {
					// todo: do not show common error if options.showFieldErrors passed
					this.notification.error(result?.message ?? options?.errorMessage ?? t("operation.error"));

					showFieldErrors(result);
				}

				if (result && result.redirectUrl) {
					this.navigation.navigate(result.redirectUrl);
				}

				return result;
			}
			catch (error) {
				this.notification.error(options?.errorMessage ?? t("operation.error"), error.message);

				if (error.response?.status == 400) {
					const result = this.convertToApiResult(<ValidationProblemDetails>error.response.data);

					showFieldErrors(result);
				}
			}
			finally {
				message();
			}
		};

		if (options?.showConfirm) {
			this.notification.confirm(
				options.confirmTitle ?? t("operation.confirm.title"),
				options.confirmContent ?? t("operation.confirm.content"),
				async () => {
					return await executeInternal();
				}
			);
		}
		else {
			return await executeInternal();
		}
	};

	confirm = async (operation: () => Promise<ApiResult>, message: string): Promise<ApiResult> => {
		return await this.execute(operation, { showConfirm: true, confirmContent: message });
	};

	confirmDelete = async (operation: () => Promise<ApiResult>): Promise<ApiResult> => {
		return await this.confirm(operation, i18next.t("operation.confirm.deleteSelected.content"));
	};

	private convertToApiResult = (details: ValidationProblemDetails): ApiResult => {

		const errors: ApiResultError[] = [];

		Object.entries(details?.errors).forEach(
			([key, value]) => errors.push({ key, messages: value })
		);

		const result: ApiResult = { success: false, errors };

		return result;
	};
}
