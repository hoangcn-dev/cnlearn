/**
 * Cuộn trang hoặc vùng chứa nội dung chính lên đầu trang hoặc đến một phần tử cụ thể
 * @param target Tùy chọn: Selector (ví dụ '#question-bank-tabs') hoặc HTMLElement để cuộn tới. Nếu không truyền, mặc định sẽ cuộn lên đầu trang.
 * @param smooth Nếu true, sẽ cuộn mượt mà (smooth). Mặc định là false (cuộn ngay lập tức).
 */
export const scrollToTop = (target?: string | HTMLElement, smooth: boolean = false) => {
  const behavior = smooth ? 'smooth' : 'auto';
  
  if (target) {
    const element = typeof target === 'string' ? document.querySelector(target) : target;
    if (element) {
      element.scrollIntoView({ behavior, block: 'start' });
      return;
    }
  }
  
  const personalContent = document.querySelector('.personal-content');
  if (personalContent) {
    personalContent.scrollTo({
      top: 0,
      behavior
    });
  } else {
    window.scrollTo({
      top: 0,
      behavior
    });
  }
};
