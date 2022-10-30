import { Button } from "antd";
import { NamePath } from "antd/lib/form/interface";
import { FieldData } from "rc-field-form/lib/interface";
import React from "react";
import { Icon, Toolbar } from ".";

interface Operations {
	remove: (index: NamePath) => void;
	move: (from: number, to: number) => void;
}

interface IProps {
	item: FieldData;
	itemIndex: number;
	itemsCount: number;
	ops: Operations;
}

export class FormListItemToolbar extends React.Component<IProps> {

	render = () => {

		const { item, itemIndex, itemsCount, ops } = this.props;

		return (
			<Toolbar float="right">
				<Button type="link" icon={Icon.MinusCircle}
					onClick={() => ops.remove(item.name)} />
				<Button type="link" icon={Icon.ArrowUp} disabled={itemIndex == 0}
					onClick={() => ops.move(itemIndex, itemIndex - 1)} />
				<Button type="link" icon={Icon.ArrowDown} disabled={itemIndex == itemsCount - 1}
					onClick={() => ops.move(itemIndex, itemIndex + 1)} />
			</Toolbar>
		);
	};
}
