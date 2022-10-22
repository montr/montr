import { Button } from "antd";
import { ButtonProps } from "antd/lib/button";
import React from "react";
import { useTranslation } from "react-i18next";
import { Icon } from ".";

export function ButtonEdit(props: ButtonProps): React.ReactElement {
	const { t } = useTranslation();

	return <Button type="primary" icon={Icon.Edit} {...props}>{props.children || t("button.edit") as string}</Button>;
}

export function ButtonSave(props: ButtonProps): React.ReactElement {
	const { t } = useTranslation();

	return <Button type="primary" icon={Icon.Check} {...props}>{props.children || t("button.save") as string}</Button>;
}

export function ButtonCancel(props: ButtonProps): React.ReactElement {
	const { t } = useTranslation();

	return <Button {...props}>{props.children || t("button.cancel") as string}</Button>;
}

export function ButtonAdd(props: ButtonProps): React.ReactElement {
	const { t } = useTranslation();

	return <Button icon={Icon.Plus} {...props}>{props.children || t("button.add") as string}</Button>;
}

export function ButtonCreate(props: ButtonProps): React.ReactElement {
	const { t } = useTranslation();

	return <Button icon={Icon.Plus} {...props}>{props.children || t("button.create") as string}</Button>;
}

export function ButtonDelete(props: ButtonProps): React.ReactElement {
	const { t } = useTranslation();

	return <Button icon={Icon.Delete} {...props}>{props.children || t("button.delete") as string}</Button>;
}

export function ButtonImport(props: ButtonProps): React.ReactElement {
	const { t } = useTranslation();

	return <Button icon={Icon.Import} {...props}>{props.children || t("button.import") as string}</Button>;
}

export function ButtonExport(props: ButtonProps): React.ReactElement {
	const { t } = useTranslation();

	return <Button icon={Icon.Export} {...props}>{props.children || t("button.export") as string}</Button>;
}

export function ButtonSelect(props: ButtonProps): React.ReactElement {
	const { t } = useTranslation();

	return <Button icon={Icon.Select} {...props}>{props.children || t("button.select") as string}</Button>;
}
