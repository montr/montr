import { ColProps } from "antd";

interface FormItemLayout {
	labelCol?: ColProps;
	wrapperCol?: ColProps;
}

interface FormItemLayoutProps {
	default: FormItemLayout;
	tail: FormItemLayout;
}

export class FormDefaults {

	public static getItemLayoutProps = (itemLayout?: "default" | "narrow"): FormItemLayoutProps => {

		if (itemLayout == "narrow") {
			return {
				default: {
					labelCol: {
						xs: { span: 24 },
						sm: { span: 12 },
						lg: { span: 8 },
					},
					wrapperCol: {
						xs: { span: 24 },
						sm: { span: 12 },
						lg: { span: 16 },
					}
				},

				tail: {
					wrapperCol: {
						xs: { offset: 0, span: 24, },
						sm: { offset: 12, span: 12, },
						lg: { offset: 8, span: 16, },
					}
				}
			};
		}

		return {
			default: {
				labelCol: {
					xs: { span: 24 },
					sm: { span: 8 },
					lg: { span: 4 },
				},
				wrapperCol: {
					xs: { span: 24 },
					sm: { span: 16 },
					lg: { span: 20 },
				}
			},

			tail: {
				wrapperCol: {
					xs: { offset: 0, span: 24, },
					sm: { offset: 8, span: 16, },
					lg: { offset: 4, span: 20, },
				}
			}
		};
	};
}
