export interface Chapter {
  id: number;
  title: string;
  content: string;
  createdAt: string;
  isVip?: boolean;
}

export interface Review {
  id: number;
  username: string;
  avatar: string;
  rating: number;
  content: string;
  level: string; // e.g. "Luyện Khí", "Trúc Cơ", "Kim Đan"
  createdAt: string;
}

export interface Novel {
  id: string;
  title: string;
  slug: string;
  author: string;
  cover: string;
  description: string;
  genres: string[];
  chaptersCount: number;
  status: 'Đang ra' | 'Hoàn thành';
  rating: number;
  views: number;
  recommendCount: number;
  chapters: Chapter[];
  reviews: Review[];
  createdAt: string;
  updatedAt: string;
  isHot?: boolean;
  isNew?: boolean;
}

export const GENRES = [
  'Tiên Hiệp',
  'Huyền Huyễn',
  'Đô Thị',
  'Khoa Huyễn',
  'Lịch Sử',
  'Võ Hiệp',
  'Dị Giới',
  'Hệ Thống',
  'Hài Hước'
];

export const mockNovels: Novel[] = [
  {
    id: 'dau-pha-thuong-khung',
    title: 'Đấu Phá Thương Khung',
    slug: 'dau-pha-thuong-khung',
    author: 'Thiên Tàm Thổ Đậu',
    cover: 'https://images.unsplash.com/photo-1543002588-bfa74002ed7e?auto=format&fit=crop&q=80&w=400',
    description: 'Đây là thế giới thuộc về Đấu Khí, không có ma pháp hoa lệ quý phái, thứ có duy nhất, chỉ là Đấu Khí đã được phát triển đến đỉnh cao! Tiêu Viêm, một thiên tài tu luyện đột nhiên biến thành phế vật, chịu đủ mọi sự khinh bỉ, sỉ nhục từ gia tộc và vị hôn thê thoái hôn. Sau khi giải phóng Dược Lão từ chiếc nhẫn mẹ để lại, cuộc đời của hắn rốt cuộc đã mở ra một trang sử hào hùng mới...',
    genres: ['Huyền Huyễn', 'Dị Giới', 'Võ Hiệp'],
    chaptersCount: 5,
    status: 'Hoàn thành',
    rating: 4.8,
    views: 1250000,
    recommendCount: 45200,
    isHot: true,
    createdAt: '2026-01-01',
    updatedAt: '10 phút trước',
    chapters: [
      {
        id: 1,
        title: 'Chương 1: Tiêu gia tam thiếu gia',
        content: `Đấu lực: Tam đoạn!

Nhìn năm chữ to to trên tấm bia đá đo lường đấu lực, thiếu niên vẻ mặt lạnh nhạt, môi hơi nhếch lên nụ cười tự giễu, nắm tay siết chặt. Bởi vì dùng sức quá độ nên móng tay đâm sâu vào da thịt, mang lại những cơn đau nhói tim gan.

"Tiêu Viêm, Đấu lực: Tam đoạn! Cấp bậc: Hạ đẳng!"

Bên cạnh tấm bia đá đo lường, một trung niên nam tử liếc mắt nhìn vào kết quả hiển thị trên bia, giọng nói lạnh lùng tuyên bố.

Lời vừa dứt, trên quảng trường rộng lớn lập tức dấy lên một hồi xôn xao sỉ nhục cùng cười nhạo.

"Tam đoạn? Quả nhiên, 'thiên tài' này vẫn đứng yên tại chỗ!"
"Ai, phế vật này thật làm mất mặt Tiêu gia chúng ta."
"Nếu không phải tộc trưởng là cha hắn, hắn đã sớm bị đuổi ra khỏi gia tộc tự sinh tự diệt rồi..."

Nghe những tiếng xì xào xung quanh truyền tới tai, thiếu niên Tiêu Viêm chỉ thở dài một tiếng, lặng lẽ quay lưng bước đi. Hắn từng là thiên tài chói sáng nhất của Ô Thản Thành, mười tuổi đã ngưng tụ Đấu Khí Khuyên, mười một tuổi đột phá Đấu Giả, trở thành tộc nhân trẻ tuổi nhất trong lịch sử gia tộc làm được điều này. Thế nhưng, ba năm trước, mọi thứ đột nhiên biến mất. Đấu khí trong cơ thể hắn biến mất một cách kỳ lạ, thực lực không ngừng thụt lùi từ Đấu Giả xuống Đấu lực Tam đoạn.

Suốt ba năm qua, hắn phải chịu đựng vô vàn sự chế giễu cùng khinh thường, từ đỉnh cao rớt xuống vực sâu.`,
        createdAt: '2026-05-20'
      },
      {
        id: 2,
        title: 'Chương 2: Đấu Khí đại lục cùng Dược Lão',
        content: `Đêm khuya thanh vắng, Tiêu Viêm nằm trên bãi cỏ sau núi, nhìn lên bầu trời đầy sao, lòng tràn ngập vẻ bất đắc dĩ. Hắn chạm tay vào chiếc nhẫn màu đen cổ phác đeo trên ngón tay, đây là di vật duy nhất mẹ hắn để lại trước khi qua đời.

"Ba năm rồi... Rốt cuộc tại sao đấu khí của ta lại biến mất?" Tiêu Viêm thì thầm tự hỏi.

"He he, tiểu oa nhi, không cần phải than thở như vậy. Ba năm qua nếu không nhờ đấu khí của ngươi tẩm bổ, lão phu cũng không thể thức tỉnh nhanh như vậy đâu."

Một giọng nói già nua đột ngột vang lên giữa hư vô.

Tiêu Viêm giật nảy mình, nhảy dựng lên, cảnh giác nhìn xung quanh: "Ai? Ai đang nói đó?"

Một làn khói trắng từ chiếc nhẫn đen nhạt bay ra, ngưng tụ thành một bóng người hư ảo trôi nổi giữa không trung. Đó là một lão giả râu tóc bạc phơ, vẻ mặt tiên phong đạo cốt nhưng nụ cười lại vô cùng xảo quyệt.

"Ngươi... ngươi là cái quỷ gì?" Tiêu Viêm trợn mắt há mồm, chỉ ngón tay run rẩy vào lão giả.

"Lão phu là ai ư? Ha ha, ngươi cứ gọi ta là Dược Lão đi." Lão giả cười tủm tỉm nói, "Tiểu tử, ba năm qua đấu khí của ngươi biến mất chính là do ta hấp thu để hồi phục linh hồn. Nhưng ngươi yên tâm, lão phu sẽ bồi thường xứng đáng cho ngươi!"`,
        createdAt: '2026-05-21'
      },
      {
        id: 3,
        title: 'Chương 3: Nạp Lan Yên Nhiên thoái hôn',
        content: `Hôm nay, Tiêu gia nghênh đón ba vị khách quý từ Vân Lam Tông. Dẫn đầu là một vị trưởng lão thực lực thâm hậu cấp Đại Đấu Sư, đi cùng là một thiếu nữ trẻ tuổi vô cùng xinh đẹp, khí chất kiêu kỳ - Nạp Lan Yên Nhiên, truyền nhân duy nhất của tông chủ Vân Lam Tông, cũng chính là vị hôn thê có hôn ước từ nhỏ với Tiêu Viêm.

Trong đại sảnh gia tộc, không khí vô cùng căng thẳng.

Sau vài câu khách sáo xã giao, trưởng lão Vân Lam Tông rốt cuộc đi vào chủ đề chính: "Tiêu tộc trưởng, lần này chúng ta tới đây, thật ra là để xin hủy bỏ hôn ước giữa Yên Nhiên và lệnh lang Tiêu Viêm."

Lời này vừa nói ra, toàn bộ đại sảnh lặng ngắt như tờ. Tộc trưởng Tiêu Chiến sắc mặt lập tức tái xanh, bàn tay siết chặt chiếc ghế gỗ làm nó xuất hiện những vết rạn nứt. Đây là sự sỉ nhục to lớn đối với Tiêu gia và Tiêu Viêm!

"Thoái hôn?" Tiêu Viêm chậm rãi bước ra, ánh mắt lạnh lùng nhìn thiếu nữ kiêu ngạo kia.

Nạp Lan Yên Nhiên cắn môi nhẹ nói: "Tiêu Viêm, ta biết chuyện này làm tổn hại đến tôn nghiêm của ngươi. Ta sẽ bồi thường cho ngươi một khỏa Tụ Khí Tán giúp ngươi đột phá Đấu Giả..."

"Tụ Khí Tán?" Tiêu Viêm nở nụ cười lạnh đầy khinh bỉ. Hắn tiến lên trước, cầm lấy giấy bút trên bàn, nhanh chóng viết xuống vài dòng chữ rồng bay phượng múa, sau đó ấn dấu tay máu lên đó rồi ném thẳng vào mặt Nạp Lan Yên Nhiên!

"Không phải ngươi muốn thoái hôn, mà là Tiêu Viêm ta hưu thê! Từ nay về sau, ngươi và ta không còn bất kỳ quan hệ nào!"

Tiêu Viêm gầm lên: "Ba mươi năm ở phía đông sông, ba mươi năm ở phía tây sông, đừng khinh thiếu niên nghèo!"`,
        createdAt: '2026-05-22'
      },
      {
        id: 4,
        title: 'Chương 4: Quyết định tu luyện',
        content: `Sau biến cố chấn động tại đại sảnh gia tộc, Tiêu Viêm trở về phòng riêng, tâm trạng vẫn chưa thể hoàn toàn bình tĩnh. Hưu thư đã viết, ước hẹn ba năm tại Vân Lam Tông cũng đã lập hạ. Ba năm sau, hắn phải tự mình đi lên Vân Lam Tông để chứng minh bản thân không phải phế vật, bảo vệ danh dự của cha và gia tộc!

"Tiểu tử, khí phách lắm!" Dược Lão từ trong nhẫn bay ra, không ngớt lời tán thưởng. "Có điều nói đi cũng phải nói lại, lấy thực lực Đấu lực tam đoạn của ngươi hiện tại, ba năm sau đi Vân Lam Tông chỉ có con đường chết. Nạp Lan Yên Nhiên kia dưới sự bồi dưỡng của Vân Lam Tông e rằng đã sắp đột phá Đấu Giả rồi."

"Dược Lão, ngài có cách giúp ta đúng không?" Tiêu Viêm ánh mắt nóng bỏng nhìn Dược Lão. Hắn biết vị linh hồn thể bí ẩn này tuyệt đối là một cường giả cái thế.

Dược Lão vuốt râu cười lớn: "Ha ha! Lão phu có một bộ công pháp vô tiền khoáng hậu tên là 'Phần Quyết'. Bộ công pháp này có thể tiến hóa thông qua việc thôn phệ các loại Dị Hỏa giữa thiên địa. Ngươi có dám tu luyện hay không?"

"Thôn phệ Dị hỏa để tiến hóa công pháp?!" Tiêu Viêm hít vào một ngụm khí lạnh. Hắn đã từng nghe qua sự kinh khủng của Dị hỏa, chỉ cần chạm nhẹ một chút cũng đủ khiến cường giả hóa thành tro bụi. Nhưng nghĩ đến ánh mắt khinh miệt của Nạp Lan Yên Nhiên và sự kỳ vọng của cha mình, hắn kiên định gật đầu: "Có gì mà không dám! Con đường tu luyện vốn là nghịch thiên mà đi!"`,
        createdAt: '2026-05-22'
      },
      {
        id: 5,
        title: 'Chương 5: Sát thủ giới luyện dược [VIP]',
        content: `Dưới sự hướng dẫn của Dược Lão, Tiêu Viêm bắt đầu hành trình trở thành một Luyện Dược Sư. Luyện dược sư là nghề nghiệp tôn quý và giàu có nhất trên Đấu Khí Đại Lục, người bình thường muốn trở thành luyện dược sư phải có thiên phú hỏa thuộc tính kèm theo một chút mộc thuộc tính để cảm ứng dược lý.

"Đốt lò! Bỏ dược liệu!" Dược Lão quát lớn.

Tiêu Viêm trán đẫm mồ hôi, dùng đấu khí hỏa hệ kích hoạt ngọn lửa bên trong dược đỉnh. Hắn cẩn thận bỏ từng gốc thảo dược vào, dùng linh hồn lực tinh vi để khống chế độ nóng của ngọn lửa nhằm chiết xuất tinh chất.

"Xèo..." Dược liệu vừa vào đỉnh lập tức bị đốt thành tro bụi.

"Nhiệt độ quá cao! Làm lại!" Dược Lão nghiêm khắc nhắc nhở.

Sau hàng chục lần thất bại liên tiếp và tiêu tốn một lượng tiền lớn mua dược thảo, cuối cùng, một làn hương thơm ngào ngạt bay ra từ dược đỉnh. Một khỏa đan dược màu đỏ tươi lấp lánh hiện ra.

"Thành công rồi! Đây là Trúc Cơ Linh Dịch!" Tiêu Viêm mừng rỡ hét lớn.

Loại linh dịch này có thể giúp người tu luyện dưới cấp Đấu Giả gia tăng tốc độ hấp thu đấu khí lên gấp nhiều lần mà không để lại di chứng. Với Trúc Cơ Linh Dịch và Phần Quyết, hành trình quật khởi của Tiêu Viêm chính thức bắt đầu!`,
        createdAt: '2026-05-22',
        isVip: true
      }
    ],
    reviews: [
      {
        id: 1,
        username: 'Hàn Lập',
        avatar: 'https://images.unsplash.com/photo-1535713875002-d1d0cf377fde?auto=format&fit=crop&q=80&w=100',
        rating: 5,
        content: 'Truyện cực kỳ hay, tình tiết thoái hôn viết rất cuốn và cảm xúc. Khẩu khí "Ba mươi năm ở phía đông sông..." thực sự làm người đọc nổi da gà. Đáng để theo dõi lâu dài!',
        level: 'Hóa Thần Kỳ',
        createdAt: '1 ngày trước'
      },
      {
        id: 2,
        username: 'Từ Tiểu Thụ',
        avatar: 'https://images.unsplash.com/photo-1570295999919-56ceb5ecca61?auto=format&fit=crop&q=80&w=100',
        rating: 4,
        content: 'Main rất có khí phách, nhưng tu luyện cực khổ quá, không có hệ thống tự động cộng điểm như ta. Chấm 4 sao khuyến khích tác giả!',
        level: 'Trúc Cơ Kỳ',
        createdAt: '2 ngày trước'
      }
    ]
  },
  {
    id: 'pham-nhan-tu-tien',
    title: 'Phàm Nhân Tu Tiên',
    slug: 'pham-nhan-tu-tien',
    author: 'Vong Ngữ',
    cover: 'https://images.unsplash.com/photo-1512820790803-83ca734da794?auto=format&fit=crop&q=80&w=400',
    description: 'Một phàm nhân bình thường, da đen người gầy, tư chất nông cạn lại bước chân vào con đường tu tiên đầy rẫy hiểm ác gian nan. Hàn Lập dựa vào tâm cơ cẩn trọng, tính toán kỹ lưỡng cùng một chiếc bình nhỏ thần kỳ có khả năng thúc chín dược thảo, từng bước tranh đấu cùng thiên địa, nghịch thiên cải mệnh để trở thành một Chí Tôn Tiên Giới...',
    genres: ['Tiên Hiệp', 'Huyền Huyễn', 'Hài Hước'],
    chaptersCount: 3,
    status: 'Hoàn thành',
    rating: 4.9,
    views: 2800000,
    recommendCount: 98100,
    isHot: true,
    createdAt: '2026-01-10',
    updatedAt: '2 giờ trước',
    chapters: [
      {
        id: 1,
        title: 'Chương 1: Sơn thôn thiếu niên Hàn Lập',
        content: `Thôn Ngũ Lý nằm ở một nơi hẻo lánh của phủ Kính Châu.

Hôm nay, trong thôn có một thiếu niên tên Hàn Lập đang ngồi dưới gốc cây đại thụ ngoài đầu thôn, ngơ ngác nhìn đàn kiến di chuyển dưới đất. Hàn Lập dáng người hơi gầy, nước da ngăm đen, diện mạo hết sức bình thường, thuộc loại ném vào đám đông sẽ ngay lập tức biến mất.

"Nhị Oa! Nhị Oa!"

Một tiếng gọi lanh lảnh từ xa truyền đến. Một tráng hán trung niên chạy hồng hộc tới: "Nhị Oa, tam thúc của ngươi từ trên trấn trở về, nói là Thất Huyền Môn đang tuyển thu đệ tử ký danh! Lần này tam thúc đề cử ngươi đi thử thời vận, nếu trúng tuyển mỗi tháng sẽ có bạc gửi về nhà đó!"

Nghe đến có bạc gửi về nhà giúp đỡ cha mẹ và muội muội, ánh mắt Hàn Lập khẽ động, hắn đứng dậy kiên định gật đầu: "Cha, con muốn đi!"

Hắn không hề biết rằng, cái gật đầu hôm nay đã thay đổi hoàn toàn vận mệnh của hắn, đem hắn kéo vào một thế giới tu tiên rộng lớn, tàn khốc nhưng cũng vô cùng đặc sắc.`,
        createdAt: '2026-05-18'
      },
      {
        id: 2,
        title: 'Chương 2: Thất Huyền Môn cùng Mặc Đại Phu',
        content: `Sau nhiều ngày đi đường vất vả, Hàn Lập cùng đám trẻ trong vùng đã tới được tổng đàn Thất Huyền Môn đóng trên Thần Thủ Cốc. Bản thân hắn vì tư chất thân thể trung bình nên không vượt qua được kỳ thi khảo hạch đệ tử chính thức. Tuy nhiên, một lão giả thần bí vẻ mặt ốm yếu tên là Mặc Đại Phu lại vô tình nhìn trúng hắn và nhận hắn làm dược đồng, dẫn về Thần Thủ Cốc.

"Hàn Lập, từ hôm nay ngươi và Lệ Phi Vũ sẽ ở lại đây học tập dược lý." Mặc Đại Phu ho khan vài tiếng, ném cho Hàn Lập một cuốn sách da thú ốm cũ: "Đây là một bộ khẩu quyết dưỡng sinh vô danh. Trong vòng nửa năm, nếu ngươi có thể tu luyện ra một luồng khí cảm nhỏ, ta sẽ thu ngươi làm đệ tử chính thức."

Hàn Lập nhận lấy cuốn sách, bắt đầu những ngày tháng tu luyện tẻ nhạt tại Thần Thủ Cốc. Hắn phát hiện bộ công pháp này vô cùng khó luyện, trong khi Lệ Phi Vũ luyện võ tiến bộ vượt bậc thì hắn vẫn dậm chân tại chỗ. Tuy nhiên nhờ sự kiên trì kinh người, Hàn Lập cuối cùng cũng cảm nhận được một luồng khí lạnh ấm áp lưu chuyển trong kinh mạch.`,
        createdAt: '2026-05-19'
      },
      {
        id: 3,
        title: 'Chương 3: Chiếc bình nhỏ thần bí',
        content: `Một hôm, Hàn Lập đi hái thuốc ở sau núi Thần Thủ Cốc. Khi bước qua một lùm cây rậm rạp, chân hắn bỗng vấp phải một vật cứng màu đen.

Tò mò cúi xuống đào lên, hắn thấy đó là một chiếc bình nhỏ màu xanh lục chỉ bằng ngón tay cái, trên thân bình có khắc những hoa văn lá cây hết sức tinh xảo, sờ vào có cảm giác mát lạnh. Trên nắp bình bị một lớp niêm phong sáp màu đen bao phủ.

Hàn Lập đem chiếc bình về phòng cất kỹ. Đêm đến, khi mặt trăng treo cao trên đỉnh đầu, một hiện tượng kỳ lạ xảy ra.

Từng luồng ánh sáng trăng trong suốt giống như sợi chỉ tơ từ trên trời giáng xuống, tụ hội xung quanh chiếc bình nhỏ. Chiếc bình hấp thu lượng ánh trăng khổng lồ, bắt đầu tỏa ra ánh hào quang màu lục rực rỡ, trên thân bình xuất hiện những giọt chất lỏng màu vàng nhạt thần bí.

"Đây rốt cuộc là bảo vật gì?" Hàn Lập kinh ngạc vạn phần. Sau nhiều lần thử nghiệm, hắn phát hiện những giọt chất lỏng màu vàng này có khả năng tăng tốc độ sinh trưởng của thảo dược lên hàng ngàn lần! Một gốc nhân sâm mười năm chỉ cần nhỏ một giọt linh dịch này vào, sáng hôm sau đã lập tức biến thành nhân sâm ngàn năm tuổi!

Có được chiếc bình này, Hàn Lập biết mình đã nắm giữ một chìa khóa vô giá để bước lên đỉnh cao tu tiên!`,
        createdAt: '2026-05-20'
      }
    ],
    reviews: [
      {
        id: 1,
        username: 'Tiêu Viêm',
        avatar: 'https://images.unsplash.com/photo-1507003211169-0a1dd7228f2d?auto=format&fit=crop&q=80&w=100',
        rating: 5,
        content: 'Hàn Lập đạo hữu tính cách cẩn thận quá mức, lúc nào cũng sẵn sàng chạy trốn (đúng chuẩn Hàn chạy nhanh!). Phong cách tu tiên thực tế, tàn khốc, không có chuyện nhảy cấp đánh bừa. Rất hay!',
        level: 'Đấu Thánh',
        createdAt: '3 ngày trước'
      }
    ]
  },
  {
    id: 'ta-co-mot-than-bi-dong',
    title: 'Ta Có Một Thân Bị Động',
    slug: 'ta-co-mot-than-bi-dong',
    author: 'Tuyệt Cảnh Đông Phương',
    cover: 'https://images.unsplash.com/photo-1531988042231-d39a9cc12a9a?auto=format&fit=crop&q=80&w=400',
    description: 'Từ Tiểu Thụ xuyên việt sang dị giới, trở thành một đệ tử ngoại môn bình thường của Thiên Tang Kiếm Môn. Hắn đạt được một hệ thống bị động thần kỳ: chỉ cần bị đánh, bị mắng, bị chấn kinh hay thậm chí chỉ cần hô hấp là có thể đạt được các kỹ năng bị động vô địch như: "Cường tráng", "Hồi phục", "Kiếm đạo tinh thông", "Hô hấp đề cao", "Hoài nghi nhân sinh"...',
    genres: ['Hệ Thống', 'Hài Hước', 'Dị Giới'],
    chaptersCount: 2,
    status: 'Đang ra',
    rating: 4.7,
    views: 890000,
    recommendCount: 32000,
    isHot: false,
    isNew: true,
    createdAt: '2026-04-15',
    updatedAt: '30 phút trước',
    chapters: [
      {
        id: 1,
        title: 'Chương 1: Hệ thống bị động của ta',
        content: `Thiên Tang Kiếm Môn, ngoại môn diễn võ trường.

"Từ Tiểu Thụ! Ngươi có gan thì đừng có trốn sau lưng hộ vệ, lên đài khiêu chiến với ta!" Một tên thiếu niên mặc áo gấm đang giận dữ hét lên.

Đứng ở dưới đài, Từ Tiểu Thụ vẻ mặt vô tội. Hắn vừa mới xuyên việt tới thế giới này được ba ngày, còn chưa hiểu rõ sự đời, thế mà đã bị cuốn vào vòng xoáy tranh đấu này.

Bỗng nhiên, một âm thanh điện tử máy móc vang lên bên tai hắn:

[Ting! Phát hiện có người khiêu khích ký chủ. Kích hoạt Hệ thống Bị động Thần cấp!]
[Ting! Đạt được kỹ năng bị động cấp sơ: 'Cường Tráng' (Giúp thân thể chịu đòn tốt gấp đôi bình thường).]
[Ting! Đạt được kỹ năng bị động cấp sơ: 'Hô Hấp' (Mỗi hơi thở đều tự động gia tăng linh lực).]

"Hả? Cái gì thế này?" Từ Tiểu Thụ trợn tròn mắt. Hắn thử hít sâu một hơi, quả nhiên cảm thấy một luồng linh khí ấm áp tự động tiến vào đan điền, chuyển hóa thành tu vi!

"Hít hà... Hít thở thôi cũng mạnh lên sao?" Từ Tiểu Thụ hưng phấn muốn ngất xỉu. Hắn quyết định: "Quyết không chủ động tu luyện! Ta chỉ cần hô hấp và bị ăn đòn để thăng cấp thôi!"`,
        createdAt: '2026-05-21'
      },
      {
        id: 2,
        title: 'Chương 2: Đến đây đi! Đánh ta đi!',
        content: `"Từ Tiểu Thụ, ngươi có nghe ta nói không hả?!" Thiếu niên áo gấm trên đài thấy Từ Tiểu Thụ cứ hít thở phình mũi trợn mắt mà không thèm trả lời, cảm thấy bị sỉ nhục ghê gớm.

"Được rồi, ta đồng ý khiêu chiến với ngươi!" Từ Tiểu Thụ thả lỏng người bước lên lôi đài, dang rộng hai tay: "Lại đây, đánh thẳng vào ngực ta này, đừng khách khí!"

Thiếu niên áo gấm giận tím mặt: "Cuồng vọng! Xem chiêu!"

Hắn rút kiếm đâm thẳng tới, linh lực hộ thể bộc phát, một chiêu kiếm mang theo kình phong sắc bén đánh trúng ngực Từ Tiểu Thụ.

"Bùm!" một tiếng trầm đục.

[Ting! Bị tấn công vật lý. Kích hoạt bị động 'Cường Tráng'. Độ thuần thục +1.]
[Ting! Kích hoạt bị động 'Hồi Phục'. Vết thương nhẹ lập tức hồi phục hoàn toàn.]

Từ Tiểu Thụ lùi lại hai bước, phủi phủi bụi trên áo, vẻ mặt tràn ngập mong đợi nhìn đối phương: "Lực đạo hơi nhẹ, ngươi chưa ăn cơm sáng à? Đánh mạnh hơn chút nữa đi!"

Thiếu niên áo gấm: "???" Hắn trố mắt nhìn thanh kiếm của mình, rồi lại nhìn Từ Tiểu Thụ không sứt mẻ một miếng da nào.

[Ting! Gây chấn kinh cho đối phương. Đạt được điểm Bị Động: +99.]

"Trời ơi, hóa ra chọc tức người khác cũng tăng điểm sao!" Từ Tiểu Thụ mở ra một cánh cửa thế giới mới đầy thú vị.`,
        createdAt: '2026-05-22'
      }
    ],
    reviews: [
      {
        id: 1,
        username: 'Bạch Tiểu Thuần',
        avatar: 'https://images.unsplash.com/photo-1500648767791-00dcc994a43e?auto=format&fit=crop&q=80&w=100',
        rating: 5,
        content: 'Ha ha, main vô sỉ đúng gu của ta! Càng bị đòn càng mạnh, cười không nhặt được mồm. Đọc giải trí cực tốt sau những giờ tu luyện căng thẳng!',
        level: 'Đại Viên Mãn',
        createdAt: '5 giờ trước'
      }
    ]
  },
  {
    id: 'quy-bi-chi-chu',
    title: 'Quỷ Bí Chi Chủ',
    slug: 'quy-bi-chi-chu',
    author: 'Ái Tiềm Thủy Đích Ô Tặc',
    cover: 'https://images.unsplash.com/photo-1516979187457-637abb4f9353?auto=format&fit=crop&q=80&w=400',
    description: 'Chu Minh Thụy tỉnh lại sau một nghi thức cầu may cổ xưa, phát hiện mình xuyên việt đến một thế giới phong cách ma ảo thời đại Cách mạng Công nghiệp hơi nước phương Tây. Tại thế giới đầy rẫy sương mù, các danh sách Ma Dược siêu phàm, những vị thần cổ xưa và quái vật kinh hoàng, hắn lấy tên là Klein Moretti, thành lập "Hội Tarot" thần bí và từng bước tìm đường trở về nhà...',
    genres: ['Khoa Huyễn', 'Dị Giới', 'Huyền Huyễn'],
    chaptersCount: 2,
    status: 'Hoàn thành',
    rating: 4.95,
    views: 3100000,
    recommendCount: 142000,
    isHot: true,
    createdAt: '2025-12-15',
    updatedAt: '1 ngày trước',
    chapters: [
      {
        id: 1,
        title: 'Chương 1: Đêm đỏ và tiếng súng',
        content: `Đau đầu, đau đầu muốn nứt ra!

Chu Minh Thụy cảm thấy linh hồn mình như vừa bị kéo ra từ một đầm lầy vô tận. Hắn cố gắng mở mắt ra, nhìn thấy ánh trăng đỏ rực như máu chiếu rọi qua cửa sổ kính.

Trên chiếc bàn gỗ mục nát trước mặt, một khẩu súng lục ổ quay màu đen xám cũ kỹ đang lẳng lặng nằm đó, nòng súng còn vương vấn một vệt khói xám nhạt, mang theo mùi thuốc súng nồng nặc.

Chu Minh Thụy giơ tay sờ lên thái dương bên phải của mình.

Ướt át, nhầy nhụa... Hắn đưa tay ra trước mắt dưới ánh trăng đỏ. Đó là máu tươi đỏ lòm!

Hắn soi mình vào tấm gương vỡ trên tường. Trong gương là một thanh niên trẻ tuổi tóc đen, mắt nâu, dáng người thư sinh gầy gò, thế nhưng trên thái dương bên phải có một lỗ đạn ghê rợn xuyên qua đại não, máu tươi vẫn đang không ngừng rỉ ra!

"Mình... mình đã chết rồi sao? Đây là đâu?"

Vô số ký ức xa lạ, hỗn loạn đột nhiên tràn vào óc hắn. Klein Moretti... Thành phố Tingen... Vương quốc Loen... Một thế giới hơi nước ma ảo đầy rẫy hiểm họa siêu phàm đang từ từ mở ra trước mắt hắn.`,
        createdAt: '2026-05-20'
      },
      {
        id: 2,
        title: 'Chương 2: Nghi thức sương xám',
        content: `Trở về nhà mình ở khu chung cư giá rẻ, Klein (bây giờ là Chu Minh Thụy) quyết định thực hiện một nghi thức tụ hội cổ xưa mà hắn từng đọc được trong một cuốn sách sưu tầm kiếp trước, hy vọng tìm được cách xuyên việt trở lại Trái Đất.

Hắn sắp xếp bốn loại nguyên liệu đơn giản ở bốn góc phòng ngủ, sau đó đi lùi từng bước và nhẩm tụng danh hiệu bằng tiếng Trung cổ:

"Phúc Sinh Huyền Hoàng Thiên Tôn..."
"Phúc Sinh Huyền Hoàng Hồng Từ..."
"Phúc Sinh Huyền Hoàng Trạch Gia..."
"Phúc Sinh Huyền Hoàng Thiên Quan..."

Bỗng nhiên, một màn sương mù màu xám trắng vô biên vô tận từ hư không tuôn ra, nhấn chìm toàn bộ căn phòng của hắn.

Thần trí Klein mơ hồ, khi tỉnh lại lần nữa, hắn thấy mình đang trôi nổi phía trên một cung điện cổ xưa hùng vĩ, lơ lửng giữa biển sương xám vô biên. Phía trước hắn là một chiếc bàn dài bằng đồng cổ kính, xung quanh có hai mươi hai chiếc ghế cao tựa lưng điêu khắc các hoa văn thần bí.

"Đây là..." Klein kinh ngạc nhìn đôi bàn tay trong suốt của mình. Hắn phát hiện mình có thể triệu hoán linh hồn của người khác đến không gian sương xám này!

Bằng cách điều động sương xám, hắn kéo hai linh hồn đang hoang mang ở thế giới thực lên đây. Đó là một thiếu nữ quý tộc kiêu kỳ và một thanh niên thủy thủ thô lỗ.

Để che giấu thân phận thật, Klein nở nụ cười nhẹ nhõm đầy thần bí, giọng nói trầm ấm vang vọng khắp cung điện đồng cổ:

"Các vị, hoan nghênh đến với Hội Tarot. Ta là... Kẻ Khờ (The Fool)."`,
        createdAt: '2026-05-21'
      }
    ],
    reviews: [
      {
        id: 1,
        username: 'Tiêu Viêm',
        avatar: 'https://images.unsplash.com/photo-1507003211169-0a1dd7228f2d?auto=format&fit=crop&q=80&w=100',
        rating: 5,
        content: 'Siêu phẩm của siêu phẩm! Thế giới xây dựng vô cùng đồ sộ, hệ thống 22 con đường danh sách ma dược cực kỳ logic. Klein Moretti diễn sâu xuất thần!',
        level: 'Đấu Thánh',
        createdAt: '2 ngày trước'
      }
    ]
  }
];
