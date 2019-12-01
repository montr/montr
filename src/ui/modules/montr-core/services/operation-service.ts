import i18next from "i18next";
import { IApiResult, IValidationProblemDetails, IApiResultError } from "../models";
import { NotificationService, NavigationService } from ".";

interface IOperationExecuteOptions {
	successMessage?: string;
	errorMessage?: string;
	showFieldErrors?: (result: IApiResult) => void;
}

export class OperationService {

	private _navigation = new NavigationService();
	private _notification = new NotificationService();

	execute = async (operation: () => Promise<IApiResult>, options?: IOperationExecuteOptions) => {

		const t = (key: string) => i18next.t(key);

		const hide = this._notification.loading(t("operation.executing"));

		const showFieldErrors = (result: IApiResult) => {
			// todo: show detailed dield errors as notification.error?
			if (options?.showFieldErrors) {
				options.showFieldErrors(result);
			}
		};

		try {
			const result = await operation();

			if (result && result.success) {
				this._notification.success(result.message ?? options?.successMessage ?? t("operation.success"));
			}
			else {
				this._notification.error(result.message ?? options?.errorMessage ?? t("operation.error"));

				showFieldErrors(result);
			}

			if (result && result.redirectUrl) {
				this._navigation.navigate(result.redirectUrl);
			}
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

	convertToApiResult = (details: IValidationProblemDetails): IApiResult => {

		const errors: IApiResultError[] = [];

		Object.entries(details?.errors).forEach(
			([key, value]) => errors.push({ key, messages: value })
		);

		const result: IApiResult = { success: false, errors };

		return result;
	};
}
