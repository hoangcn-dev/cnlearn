import { createVNode, render } from 'vue';
import type { VNode, AppContext } from 'vue';
import BaseDialog from '@/components/BaseDialog.vue';

export interface DialogOptions {
  title?: string;
  content?: string | VNode | (() => VNode);
  width?: number | string;
  closable?: boolean;
  maskClosable?: boolean;
  showCancel?: boolean;
  showOk?: boolean;
  cancelText?: string;
  okText?: string;
  onOk?: () => void | Promise<void>;
  onCancel?: () => void;
  appContext?: AppContext;
}

export const showDialog = (options: DialogOptions) => {
  const container = document.createElement('div');
  document.body.appendChild(container);

  let isClosing = false;

  const removeDialog = () => {
    if (isClosing) return;
    isClosing = true;
    
    // Đợi hiệu ứng đóng modal của Ant Design kết thúc (khoảng 300ms)
    setTimeout(() => {
      render(null, container);
      if (container.parentNode) {
        container.parentNode.removeChild(container);
      }
    }, 300);
  };

  const vnode = createVNode(BaseDialog, {
    visible: true,
    title: options.title,
    width: options.width,
    closable: options.closable,
    maskClosable: options.maskClosable,
    showCancel: options.showCancel,
    showOk: options.showOk,
    cancelText: options.cancelText,
    okText: options.okText,
    'onUpdate:visible': (val: boolean) => {
      if (!val) {
        removeDialog();
      }
    },
    onCancel: () => {
      if (options.onCancel) {
        options.onCancel();
      }
      removeDialog();
    },
    onOk: async () => {
      if (options.onOk) {
        // Hỗ trợ async/await trong nút OK
        try {
          await options.onOk();
        } catch (error) {
          console.error('Lỗi khi thực hiện onOk dialog:', error);
        }
      }
      removeDialog();
    }
  }, {
    // Render nội dung truyền vào qua slot default
    default: () => {
      if (typeof options.content === 'function') {
        return options.content();
      }
      return options.content;
    }
  });

  // Kế thừa context hiện tại nếu có (để dùng chung store, i18n, components...)
  if (options.appContext) {
    vnode.appContext = options.appContext;
  }

  render(vnode, container);
};
