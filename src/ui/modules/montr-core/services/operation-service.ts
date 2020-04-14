import i18next from "i18next";
import { IApiResult, IValidationProblemDetails, IApiResultError } from "../models";
import { NotificationService, NavigationService } from ".";

interface IOperationExecuteOptions {
	successMessage?: string;
	errorMessage?: string;
	showFieldErrors?: (result: IApiResult) => void;
	showConfirm?: boolean;
	confirmTitle?: string;
	confirmContent?: string;
}

export class OperationService {

	private _navigation = new NavigationService();
	private _notification = new NotificationService();

	// todo: remove return value
	execute = async (operation: () => Promise<IApiResult>, options?: IOperationExecuteOptions): Promise<IApiResult> => {

		const t = (key: string) => i18next.t(key);

		const showFieldErrors = (result: IApiResult) => {
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

		const executeInternal = async (): Promise<IApiResult> => {

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
					const result = this.convertToApiResult(<IValidationProblemDetails>error.response.data);

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
					return await executeInternal();
				}
			);
		}
		else {
			return await executeInternal();
		}
	};

	convertToApiResult = (details: IValidationProblemDetails): IApiResult => {

		const errors: IApiResultError[] = [];

		Object.entries(details?.errors).forEach(
			([key, value]) => errors.push({ key, messages: value })
		);

		const result: IApiResult = { success: false, errors };

		return result;
	};
}
