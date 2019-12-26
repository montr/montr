import React from "react";
import { useTranslation } from "react-i18next";
import { Button } from "antd";
import { ButtonProps } from "antd/lib/button";
import { Icon } from ".";

export function ButtonAdd(props: ButtonProps) {
	const { t } = useTranslation();

	return (
		<Button icon={Icon.Plus} {...props}>{t("button.add")}</Button >
	);
}
