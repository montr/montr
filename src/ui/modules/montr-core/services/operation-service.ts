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

	private _navigation = new NavigationService();
	private _notification = new NotificationService();

	execute = async (operation: () => Promise<ApiResult>, options?: OperationExecuteOptions): Promise<void> => {

		const t = (key: string) => i18next.t(key);

		const showFieldErrors = (result: ApiResult) => {
			// todo: show detailed field errors as notification.error?
			if (options?.showFieldErrors) {
				options.showFieldErrors(result);
			}
			else if (result?.errors) {
				result.errors.forEach(x => {
					x.messages.forEach(e => {
						this._notification.error(e);
					});
				});
			}
		};

		const executeInternal = async (): Promise<ApiResult> => {

			const hide = this._notification.loading(t("operation.executing"));

			try {
				const result = await operation();

				if (result && result.success) {
					this._notification.success(result.message ?? options?.successMessage ?? t("operation.success"));
				}
				else {
					// todo: do not show common error if options.showFieldErrors passed
					this._notification.error(result?.message ?? options?.errorMessage ?? t("operation.error"));

					showFieldErrors(result);
				}

				if (result && result.redirectUrl) {
					this._navigation.navigate(result.redirectUrl);
				}

				return result;
			}
			catch (error) {
				this._notification.error(options?.errorMessage ?? t("operation.error"), error.message);

				if (error.response?.status == 400) {
					const result = this.convertToApiResult(<ValidationProblemDetails>error.response.data);

					showFieldErrors(result);
				}
			}
			finally {
				hide();
			}
		};

		if (options?.showConfirm) {
			this._notification.confirm(
				options.confirmTitle ?? t("operation.confirm.title"),
				options.confirmContent,
				async () => {
					await executeInternal();
				}
			);
		}
		else {
			await executeInternal();
		}
	};

	convertToApiResult = (details: ValidationProblemDetails): ApiResult => {

		const errors: ApiResultError[] = [];

		Object.entries(details?.errors).forEach(
			([key, value]) => errors.push({ key, messages: value })
		);

		const result: ApiResult = { success: false, errors };

		return result;
	};
}
