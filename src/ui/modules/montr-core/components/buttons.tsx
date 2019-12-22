import React from "react";
import { useTranslation } from "react-i18next";
import { Button } from "antd";

interface IButtonProps {
	onClick?: React.MouseEventHandler<HTMLElement>;
}

export function ButtonAdd({ onClick }: IButtonProps) {
	const { t } = useTranslation();

	return (
		<Button icon="plus" onClick={onClick}>{t("button.add")}</Button >
	);
}
