import React from "react";
import { useTranslation } from "react-i18next";
import { Button } from "antd";
import { ButtonProps } from "antd/lib/button";

export function ButtonAdd(props: ButtonProps) {
	const { t } = useTranslation();

	return <Button icon="plus" {...props}>{t("button.add")}</Button>;
}

export function ButtonCreate(props: ButtonProps) {
	const { t } = useTranslation();

	return <Button icon="plus" {...props}>{t("button.create")}</Button>;
}

export function ButtonDelete(props: ButtonProps) {
	const { t } = useTranslation();

	return <Button icon="delete" {...props}>{t("button.delete")}</Button>;
}

export function ButtonImport(props: ButtonProps) {
	const { t } = useTranslation();

	return <Button icon="import" {...props}>{t("button.import")}</Button>;
}

export function ButtonExport(props: ButtonProps) {
	const { t } = useTranslation();

	return <Button icon="export" {...props}>{t("button.export")}</Button>;
}
