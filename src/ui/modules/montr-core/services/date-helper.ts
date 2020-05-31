export class DateHelper {

	public static fromUtcDateToDate = (value: string | Date): Date => {
		if (value) {
			const utcDate = (typeof value === "string") ? new Date(Date.parse(value)) : value;

			const date = new Date(Date.UTC(
				utcDate.getFullYear(), utcDate.getMonth(), utcDate.getDate(),
				utcDate.getHours(), utcDate.getMinutes(), utcDate.getSeconds(), utcDate.getMinutes()));
			return date;
		}

		return null;
	};

	public static toLocaleDateString = (text: string | Date): string => {
		const date = DateHelper.fromUtcDateToDate(text);

		return date ? date.toLocaleDateString() : null;
	};

	public static toLocaleTimeString = (text: string | Date): string => {
		const date = DateHelper.fromUtcDateToDate(text);

		return date ? date.toLocaleTimeString() : null;
	};

	public static toLocaleDateTimeString = (text: string | Date): string => {
		const date = DateHelper.fromUtcDateToDate(text);

		return date ? `${date.toLocaleDateString()} ${date.toLocaleTimeString()}` : null;
	};

}
