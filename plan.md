# Kế hoạch triển khai hai chức năng hóa đơn bán hàng

Đã xem luồng lập hóa đơn bán lẻ và bán sỉ, cách lưu phiếu bán cùng chi tiết, và cấu hình dự án WinForms .NET Framework 4.8. Phần lập hóa đơn hiện được chia giữa hai form gần giống nhau: [frmBanLe.cs](http://frmBanLe.cs) và [frmBanSi.cs](http://frmBanSi.cs). Hóa đơn và tổng tiền được lưu qua [BanLeRepository.cs](http://DataLayer/BanLeRepository.cs); thông tin phiếu bán được ánh xạ trong [PhieuBan.cs](http://BusinessObject/PhieuBan.cs).

## Giả định nghiệp vụ để lập kế hoạch

- Dịch vụ phát sinh và vận chuyển là các khoản cộng vào hóa đơn, có thể có nhiều khoản dịch vụ.  
- Chiết khấu/giảm giá được áp dụng ở cấp hóa đơn, theo **phần trăm.**  
- Tổng thanh toán \= tiền hàng \+ dịch vụ \+ vận chuyển − giảm giá; tiền còn nợ \= tổng thanh toán − đã trả.  
- Hai chức năng cần hoạt động cho cả hóa đơn bán lẻ và bán sỉ, đồng thời hiển thị lại khi mở/in hóa đơn cũ.  
- Chưa có quy tắc cụ thể về thời điểm tính chiết khấu (trước hay sau phí) hoặc giới hạn phần trăm. Cần chốt các quy tắc này trước khi triển khai để tránh sai lệch nghiệp vụ.

## Hướng áp dụng design pattern

Không nên ép cả mười mẫu vào một lớp. Kế hoạch sử dụng chúng tại nơi phù hợp, theo định nghĩa trong [Refactoring.Guru Design Patterns Catalog](https://refactoring.guru/design-patterns/catalog).

### 1\. Dịch vụ phát sinh và chi phí vận chuyển

| Mẫu | Dự kiến áp dụng |
| :---- | :---- |
| **Factory Method** | Tạo loại dòng phụ phí phù hợp, chẳng hạn dòng dịch vụ hoặc dòng vận chuyển, qua giao diện chung. |
| **Composite** | Xem tập hợp dòng hàng, dịch vụ và vận chuyển như một cấu phần hóa đơn để tính tổng nhất quán. |
| **Decorator** | Bọc/tăng cường một dòng hoặc cấu phần tính tiền để gắn phụ phí hoặc thông tin tính toán mà không sửa luồng tính hàng tồn kho. |
| **Observer** | Khi danh sách dòng hoặc số tiền thay đổi, cập nhật thành tiền, tổng thanh toán và còn nợ. |
| **Mediator** | Điều phối tương tác giữa form, danh sách dòng, bộ tính tổng và bộ lưu, tránh để các thành phần gọi trực tiếp lẫn nhau. |

### 2\. Chiết khấu và giảm giá cấp hóa đơn

| Mẫu | Dự kiến áp dụng |
| :---- | :---- |
| **Builder** | Dựng kết quả tính hóa đơn từ các thành phần: tiền hàng, dịch vụ, vận chuyển, loại/giá trị giảm và số đã trả. |
| **Facade** | Cung cấp một điểm gọi đơn giản cho form để tính và kiểm tra hóa đơn, thay vì gọi nhiều lớp nghiệp vụ. |
| **Strategy** | Thay đổi thuật toán giảm giá giữa giảm theo phần trăm và giảm số tiền cố định. |
| **Chain of Responsibility** | Chạy lần lượt các kiểm tra như giá trị giảm hợp lệ, không vượt mức cho phép, tổng hóa đơn không âm, số đã trả hợp lệ. |
| **Template Method** | Chuẩn hóa các bước tính hóa đơn, cho phép thay đổi các bước phù hợp nếu bán lẻ và bán sỉ có quy tắc riêng. |

## Các giai đoạn thực hiện

1. **Chốt quy tắc nghiệp vụ và dữ liệu**  
   Trường hợp : Công ty có sẵn container,xe máy,...  
   - Chốt loại và tên khoản dịch vụ, cách nhập phí vận chuyển, cho phép một dòng vận chuyển ( ship)  
   - Chốt cách áp dụng chiết khấu (\<20%),  
   - Giảm giá , giới hạn giá trị, **quy tắc làm tròn và thứ tự tính so với phí.**  
   - Không cho sửa hóa đơn cũ và cách xử lý các hóa đơn đã tồn tại**( tự AI xử lý)**.

	\-\>Chốt những nghiệp vụ ở trên:  
	\-Loại dịch vụ: phí bốc xếp(bán sỉ) , phí đóng gói (bán sỉ) , tự nhập dịch vụ khác nếu có  
            \* Giao hàng (Đây là lựa chọn phụ : Chọn giữa Giao hàng và Lấy trực tiếp (Mặc định) → Nếu chọn giao thì tính theo phí dưới đây :  
	\- Phí vận chuyển (quy định chung: cho nhân viên nhập khoảng cách (KM)  
	\+ Bán lẻ: dưới 3km là 10k vnd, từ 3-5km là 15k vnd, từ 5-10km thì 25k vnd, từ 10-15km 35k vnd, trên 15km thì cho nhập tiền phí vận chuyển  
\+Bán sỉ (tính theo giá trị đơn hàng \+ khoảng cách): Dưới 5 triệu dưới 10km là 100k vnd, trên 10km là 200k vnd; 5-10 triệu dưới 10km là 50k vnd, trên 10km là 150k vnd; 10-20 triệu dưới 10km là miễn phí, trên 10km là 100k vnd; trên 20 triệu duới 10km là miễn phí, trên 10km là miễn phí.  
           \- Phần chiết khấu và giảm giá: sẽ có 1 combobox cho chọn 1 trong 2 là giảm giá/chiết khấu, sau đó nhân viên nhập % giảm giá/chiết khẩu

2. **Thiết kế mô hình tính hóa đơn**  
     
   - Tách việc tính tổng khỏi logic WinForms hiện tại.  
   - Định nghĩa dòng tiền hàng và dòng phụ phí sao cho phụ phí không làm thay đổi tồn kho, giá vốn hoặc phân bổ lô.  
   - Xây dựng lớp/contract cho chiến lược giảm giá và các bước kiểm tra.  
   - Thêm bộ điều phối sự kiện tính lại khi thay đổi dòng, phí, giảm giá hoặc số đã trả.

   

3. **Cập nhật giao diện lập hóa đơn**  
     
   - Bổ sung vùng nhập dịch vụ/vận chuyển và vùng chọn loại/giá trị giảm giá.  
   - Hiển thị riêng tiền hàng, phụ phí, giảm giá, tổng thanh toán và còn nợ.  
   - Khóa/mở trường theo trạng thái thêm mới, chỉnh sửa, xem như các trường hiện tại.  
   - Đồng bộ trải nghiệm giữa form bán lẻ và bán sỉ; cân nhắc trích phần dùng chung để tránh nhân đôi logic.

   

4. **Cập nhật lưu và đọc hóa đơn**  
     
   - Lưu và tải lại các khoản phụ phí, giảm giá cùng phiếu.  
   - Bảo đảm lưu phiếu, chi tiết hàng và khoản phụ phí trong cùng transaction.  
   - Cập nhật mô hình [PhieuBan.cs](http://BusinessObject/PhieuBan.cs), controller và lớp truy cập dữ liệu tương ứng.  
   - Rà soát xóa/sửa phiếu, nợ khách hàng, danh sách phiếu và báo cáo để dùng đúng tổng thanh toán.

   

5. **Cập nhật mẫu in**  
     
   - Điều chỉnh [frmInPhieuBan.cs](http://frmInPhieuBan.cs) và report [rptPhieuBan.rdlc](http://Report/rptPhieuBan.rdlc) để thể hiện phụ phí, chiết khấu và tổng sau giảm.  
   - Kiểm tra dữ liệu hóa đơn cũ không có các trường mới vẫn đọc/in được.

   

6. **Kiểm thử**  
     
   - Kiểm thử không có phụ phí/giảm giá, nhiều dịch vụ, phí vận chuyển bằng 0\.  
   - Kiểm thử giảm giá phần trăm và số tiền, giá trị biên và dữ liệu không hợp lệ.  
   - Kiểm thử chỉnh sửa, tải lại, xóa, in hóa đơn, tính tiền đã trả/còn nợ và lưu transaction.  
   - Kiểm thử cả bán lẻ và bán sỉ, đảm bảo không ảnh hưởng tồn kho và giá vốn.

## Các file dự kiến liên quan

**Sẽ sửa:**

- [frmBanLe.cs](http://frmBanLe.cs), [frmBanLe.Designer.cs](http://frmBanLe.Designer.cs)  
- [frmBanSi.cs](http://frmBanSi.cs), [frmBanSi.Designer.cs](http://frmBanSi.Designer.cs)  
- [BusinessObject/PhieuBan.cs](http://BusinessObject/PhieuBan.cs)  
- [Controller/PhieuBanController.cs](http://Controller/PhieuBanController.cs)  
- [DataLayer/BanLeRepository.cs](http://DataLayer/BanLeRepository.cs)  
- [frmInPhieuBan.cs](http://frmInPhieuBan.cs)  
- [Report/rptPhieuBan.rdlc](http://Report/rptPhieuBan.rdlc)  
- [CuahangNongDuoc.csproj](http://CuahangNongDuoc.csproj) để khai báo các lớp mới trong project kiểu cũ.

**Dự kiến tạo mới** (tên có thể điều chỉnh khi bắt đầu triển khai):

- Các lớp mô hình dòng phụ phí/dịch vụ và thành phần tính hóa đơn.  
- Các lớp Factory Method, Composite, Decorator, Observer/Mediator cho luồng phụ phí.  
- Các lớp Builder, Facade, Strategy, Chain of Responsibility và Template Method cho luồng chiết khấu/tính tổng.  
- Có thể cần lớp repository/factory riêng cho các khoản phụ phí nếu tách dữ liệu khỏi phiếu bán.

## Cơ sở dữ liệu

Khả năng cao cần sửa cơ sở dữ liệu để lưu chi tiết dịch vụ/vận chuyển và giá trị/loại chiết khấu. Phương án dự kiến là thêm bảng chi tiết phụ phí liên kết với `PHIEU_BAN`, đồng thời thêm các cột chiết khấu vào `PHIEU_BAN` (hoặc lưu như dòng phụ phí âm nếu quy tắc và báo cáo cho phép). Cần kiểm tra schema thực tế trước khi quyết định cấu trúc cuối cùng. **Tôi sẽ chỉ ghi chú câu lệnh/cột/bảng cần thêm để bạn tự cập nhật cơ sở dữ liệu, không tự sửa database.**

Đây là kế hoạch, chưa thay đổi mã nguồn hay cơ sở dữ liệu.