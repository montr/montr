import React from "react";
import { Button } from "antd";
import { Toolbar, Icon } from ".";

interface FieldData {
	name: number;
	key: number;
	fieldKey: number;
}

interface Operations {
	remove: (index: number) => void;
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
