import React from "react";
import { useTranslation } from "react-i18next";
import { Button } from "antd";
import { ButtonProps } from "antd/lib/button";
import { Icon } from ".";

export function ButtonSave(props: ButtonProps) {
	const { t } = useTranslation();

	return <Button type="primary" icon={Icon.Check} {...props}>{props.children || t("button.save")}</Button>;
}

export function ButtonCancel(props: ButtonProps) {
	const { t } = useTranslation();

	return <Button {...props}>{props.children || t("button.cancel")}</Button>;
}

export function ButtonAdd(props: ButtonProps) {
	const { t } = useTranslation();

	return <Button icon={Icon.Plus} {...props}>{props.children || t("button.add")}</Button>;
}

export function ButtonCreate(props: ButtonProps) {
	const { t } = useTranslation();

	return <Button icon={Icon.Plus} {...props}>{props.children || t("button.create")}</Button>;
}

export function ButtonDelete(props: ButtonProps) {
	const { t } = useTranslation();

	return <Button icon={Icon.Delete} {...props}>{props.children || t("button.delete")}</Button>;
}

export function ButtonImport(props: ButtonProps) {
	const { t } = useTranslation();

	return <Button icon={Icon.Import} {...props}>{props.children || t("button.import")}</Button>;
}

export function ButtonExport(props: ButtonProps) {
	const { t } = useTranslation();

	return <Button icon={Icon.Export} {...props}>{props.children || t("button.export")}</Button>;
}
