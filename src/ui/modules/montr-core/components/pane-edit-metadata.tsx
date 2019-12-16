import * as React from "react";
import { useTranslation } from "react-i18next";
import { Button, Spin, Icon, Divider, Input, Card, Form } from "antd";
import { Toolbar } from "./toolbar";
import { IDataField } from "@montr-core/models";
import { FormDefaults } from ".";

interface IState {
	loading: boolean;
	fields?: IDataField[];
}

export function PaneEditMetadata() {

	const { t } = useTranslation(),
		[state, setState] = React.useState<IState>({ loading: true });

	React.useEffect(() => {
		async function fetchData() {
			const fields: IDataField[] = [
				{ key: "fullName", name: "Полное наименование", type: "string" },
				{ key: "shortName", name: "Сокращенное наименование", type: "string" },
				{ key: "address", name: "Адрес в пределах места пребывания", type: "address" },
				{ key: "okved", name: "Код(ы) ОКВЭД", type: "classifier" },
				{ key: "inn", name: "ИНН", type: "string" },
				{ key: "kpp", name: "КПП", type: "string" },
				{ key: "dp", name: "Дата постановки на учет в налоговом органе", type: "date" },
				{ key: "ogrn", name: "ОГРН", type: "string" },
				{ key: "is_msp", name: "Участник закупки является субъектом малого предпринимательства", type: "boolean" },
			];

			setState({ loading: false, fields: fields });
		}

		fetchData();

		return async () => {
			// await metadataService.abort();
		};
	}, []);

	const { loading, fields } = state;

	return (
		<Spin spinning={loading}>
			<Toolbar>
				<Button>{t("button.add")}</Button>
			</Toolbar>

			<Divider />

			{fields && fields.map(x =>
				<Card key={x.key} style={{ marginBottom: 8 }}
					actions={[
						<Icon type="arrow-up" key="arrow-up" />,
						<Icon type="arrow-down" key="arrow-down" />,
						<Icon type="setting" key="setting" />,
						<Icon type="edit" key="edit" />,
						<Icon type="ellipsis" key="ellipsis" />,
					]}>
					{/* <Card.Meta
							avatar={<svg style={{ width: 36, height: 36 }} aria-hidden="true" focusable="false" preserveAspectRatio="xMidYMid meet" viewBox="0 0 24 24">
								<path d="M5 17v2h14v-2H5zm4.5-4.2h5l.9 2.2h2.1L12.75 4h-1.5L6.5 15h2.1l.9-2.2zM12 5.98L13.87 11h-3.74L12 5.98z" fill="#626262" />
							</svg>} /> */}

					<Form layout="horizontal">
						<Form.Item label="Key" {...FormDefaults.formItemLayout}>
							<Input value={x.key} />
						</Form.Item>
						<Form.Item label="Name" {...FormDefaults.formItemLayout}>
							<Input.TextArea value={x.name} />
						</Form.Item>
					</Form>

				</Card>
			)}

		</Spin>
	);
}
