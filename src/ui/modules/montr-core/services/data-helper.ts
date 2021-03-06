export class DataHelper {
	public static indexer = (obj: any, is: string | string[], value: any): any => {
		if (typeof is == "string")
			return DataHelper.indexer(obj, is.split('.'), value);
		else if (is.length == 1 && value !== undefined)
			return obj ? obj[is[0]] = value : null;
		else if (is.length == 0)
			return obj;
		else
			return DataHelper.indexer(obj ? obj[is[0]] : null, is.slice(1), value);
	};
}
