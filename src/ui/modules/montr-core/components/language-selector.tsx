import { Dropdown } from "antd";
import { i18n } from "i18next";
import * as React from "react";
import { Translation } from "react-i18next";
import { DataMenu, Icon } from ".";
import { IIndexer, IMenu } from "../models";

export const LanguageSelector = () => {

	// todo: load from server
	const langs: IIndexer = {
		"en": { title: "English" },
		"ru": { title: "русский" },
	};

	function getOverlayItems(i18n: i18n): IMenu[] {
		return Object.keys(langs).map(lng => ({
			name: langs[lng].title,
			onClick: () => i18n.changeLanguage(lng)
		}));
	}

	return (
		<Translation>
			{(t, { i18n }) => (
				<Dropdown
					trigger={["click"]}
					overlay={<DataMenu tail={getOverlayItems(i18n)} />}>
					<a className="ant-dropdown-link">
						{Icon.Global} {langs[i18n.language].title} {Icon.Down}
					</a>
				</Dropdown>
			)}
		</Translation>
	);
};
