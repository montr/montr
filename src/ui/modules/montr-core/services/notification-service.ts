import { message, Modal, notification } from "antd";
import { MessageType } from "antd/lib/message";

declare type Content = React.ReactNode | string;
declare type Duration = number | (() => void);
declare type OnClose = () => void;

export class NotificationService {

	static initialize(): void {
		notification.config({
			// placement: "bottomRight"
		});
	}

	public info = (content: Content, duration?: Duration, onClose?: OnClose): MessageType => {
		return message.info(content, duration, onClose);
	};

	public success = (content: Content, duration?: Duration, onClose?: OnClose): MessageType => {
		return message.success(content, duration, onClose);
	};

	public error = (message: React.ReactNode, description?: React.ReactNode): void => {
		notification.error({ message, description });
	};

	public warning = (content: Content, duration?: Duration, onClose?: OnClose): MessageType => {
		return message.warning(content, duration, onClose);
	};

	public loading = (content: Content, duration?: Duration, onClose?: OnClose): MessageType => {
		return message.loading(content, duration, onClose);
	};

	public confirm = (title: Content, content: Content, onOk?: (...args: any[]) => any, onCancel?: (...args: any[]) => any): void => {
		Modal.confirm({ title, content, onOk, onCancel });
	};
}

NotificationService.initialize();
