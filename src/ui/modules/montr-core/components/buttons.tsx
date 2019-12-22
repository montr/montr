import React from "react";
import { useTranslation } from "react-i18next";
import { Button } from "antd";
import { Icon } from ".";

interface IButtonProps {
	onClick?: React.MouseEventHandler<HTMLElement>;
}

export function ButtonAdd({ onClick }: IButtonProps) {
	const { t } = useTranslation();

	return (
		<Button icon={Icon.Plus} onClick={onClick}>{t("button.add")}</Button >
	);
}
