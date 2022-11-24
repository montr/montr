import { NamePath } from "antd/es/form/interface";

export function extendNamePath(name: NamePath, extra: (string | number)[]): (string | number)[] {

	const result: (string | number)[] = [];

	if (typeof name == "string") {
		result.push(name as string);
	}
	else if (typeof name == "number") {
		result.push(name as number);
	}
	else {
		(name as (string | number)[]).forEach(x => result.push(x));
	}

	extra.forEach(x => result.push(x));

	return result;
}

export function joinNamePath(name: NamePath): string {
	if (typeof name == "string" || typeof name == "number") {
		return name.toString();
	}

	return (name as (string | number)[]).join("_");
}
