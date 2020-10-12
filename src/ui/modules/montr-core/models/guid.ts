// https://gist.github.com/emptyother/1fd97db034ef848f38eca3354fa9ee90

export class Guid {
	public static new(): Guid {
		return new Guid('xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, c => {
			const r = Math.random() * 16 | 0;
			const v = (c == 'x') ? r : (r & 0x3 | 0x8);
			return v.toString(16);
		}));
	}

	public static get empty(): string {
		return '00000000-0000-0000-0000-000000000000';
	}

	public static isValid(str: string): boolean {
		const validRegex = /^[0-9a-f]{8}-[0-9a-f]{4}-[1-5][0-9a-f]{3}-[89ab][0-9a-f]{3}-[0-9a-f]{12}$/i;
		return validRegex.test(str);
	}

	private value: string = Guid.empty;

	constructor(value?: string) {
		if (value && Guid.isValid(value)) {
			this.value = value;
		}
	}

	public toString(): string {
		return this.value;
	}

	public toJSON(): string {
		return this.value;
	}
}
