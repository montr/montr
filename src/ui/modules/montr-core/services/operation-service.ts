import i18next from "i18next";
import { IApiResult } from "../models";
import { NotificationService } from ".";

export class OperationService {

	private _notification = new NotificationService();

	public execute = async (operation: () => Promise<IApiResult>) => {

		const hide = this._notification.loading(i18next.t("operation.executing"));

		try {
			const result = await operation();

			if (result.message) {
				if (result.success) {
					this._notification.success(result.message);
				}
				else {
					this._notification.error(result.message);
				}
			}
			else {
				if (result.success) {
					this._notification.success(i18next.t("operation.success"));
				}
				else {
					this._notification.error(i18next.t("operation.error"));
				}
			}
		}
		catch (error) {
			this._notification.error(i18next.t("operation.error"), error.message);
		}
		finally {
			hide();
		}
	};
}
