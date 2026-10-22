# Kế hoạch triển khai hai chức năng hóa đơn bán hàng

## Phạm vi đã xác nhận

Ứng dụng là WinForms .NET Framework 4.8. Luồng lập hóa đơn bán lẻ/bán sỉ hiện nằm ở [frmBanLe.cs](http://frmBanLe.cs) và [frmBanSi.cs](http://frmBanSi.cs), với phần lưu tồn kho/phiếu bán tập trung đáng kể trong [BanLeRepository.cs](http://DataLayer/BanLeRepository.cs). Hóa đơn bán hàng dùng chung bảng `PHIEU_BAN` và chi tiết hàng hóa dùng `CHI_TIET_PHIEU_BAN`.

### Nghiệp vụ đã chốt

- Áp dụng cho cả bán lẻ và bán sỉ; hóa đơn đã lập chỉ được xem/in, **cho sửa hóa đơn cũ (nhưng chỉ cho sửa phần đã trả và không được sửa số tiền đã trả nhỏ hơn lần chỉnh sửa trước đó)**.  
    
- Giao hàng là lựa chọn không bắt buộc; mặc định **lấy trực tiếp**. Nếu chọn giao hàng, nhân viên nhập khoảng cách (km) và hệ thống tính phí vận chuyển theo bảng giá bên dưới.  
    
- Dịch vụ phát sinh:  
    
  - Bán sỉ: phí bốc xếp, phí đóng gói.  
  - Có thể tự nhập dịch vụ khác nếu phát sinh.  
  - Cho phép tối đa một dòng vận chuyển (ship) trên mỗi hóa đơn.


- Chiết khấu/giảm giá: combobox chọn một trong hai loại **Giảm giá** hoặc **Chiết khấu**, sau đó nhập tỷ lệ phần trăm. Tỷ lệ phải **nhỏ hơn 20%**.  
    
  Phí vận chuyển bán lẻ:  
  - Dưới 3 km: 10.000 VND.  
  - Từ 3 đến dưới 5 km: 15.000 VND.  
  - Từ 5 đến dưới 10 km: 25.000 VND.  
  - Từ 10 đến 15 km: 35.000 VND.  
  - Trên 15 km: nhân viên nhập phí.


  


- Phí vận chuyển bán sỉ, căn cứ giá trị đơn hàng và khoảng cách:


| Giá trị đơn hàng | Dưới 10 km | Từ 10 km trở lên |
| :---- | ----: | ----: |
| Dưới 5.000.000 VND | 100.000 VND | 200.000 VND |
| Từ 5.000.000 đến dưới 10.000.000 VND | 50.000 VND | 150.000 VND |
| Từ 10.000.000 đến dưới 20.000.000 VND | Miễn phí | 100.000 VND |
| Từ 20.000.000 VND trở lên | Miễn phí | Miễn phí |

### Điểm cần ghi nhận trước khi code

Các quyết định chính đã được chốt ở trên. Những quy tắc sau chưa được nêu rõ trong phần chốt, vì vậy khi triển khai cần dùng giả định thống nhất và ghi rõ trong giao diện/kiểm thử:

- Công thức phí bán lẻ hiện chưa định nghĩa khoảng cách âm/0; đề xuất chỉ cho phép khoảng cách dương khi chọn giao hàng.  
- Với bán lẻ, mốc 15 km được hiểu là thuộc bậc 10–15 km (35.000 VND), lớn hơn 15 km thì nhập phí.  
- Với bán sỉ, mốc 10 km thuộc nhóm từ 10 km trở lên; các mốc giá trị đơn hàng 5/10/20 triệu thuộc bậc bắt đầu tại mốc đó.  
- Chưa chốt cơ sở tính giá trị đơn hàng để tra phí sỉ (đề xuất tiền hàng trước phí và trước giảm giá), thứ tự áp dụng chiết khấu so với dịch vụ/vận chuyển và quy tắc làm tròn số tiền giảm. Cần thống nhất các điểm này để không phát sinh kết quả tính khác nhau. **Chốt:** Phí ship sỉ tra theo tiền hàng trước các khoản phí và giảm giá; giảm giá tính trên tiền hàng; làm tròn đến đồng gần nhất   
- Tỷ lệ dưới 20% được hiểu là `0 <= tỷ lệ < 20`; không cho nhập số âm.  
- Cần xác nhận loại “Giảm giá” và “Chiết khấu” hiện chỉ khác tên/ý nghĩa trên chứng từ hay có quy tắc tính khác nhau. Tạm thời dùng cùng công thức phần trăm, lưu lại lựa chọn để hiển thị/in.

## Hướng áp dụng design pattern

Áp dụng theo đúng ý nghĩa các mẫu trong [Refactoring.Guru Design Patterns Catalog](https://refactoring.guru/design-patterns/catalog), không tạo lớp chỉ để gắn tên pattern:

### 1\. Dịch vụ phát sinh và chi phí vận chuyển

| Mẫu | Vị trí áp dụng dự kiến |
| :---- | :---- |
| **Factory Method** | Tạo loại dòng phụ phí (bốc xếp, đóng gói, dịch vụ tự nhập, vận chuyển) thông qua giao diện chung. |
| **Composite** | Tính tổng hóa đơn trên cấu trúc gồm nhóm tiền hàng và các dòng phụ phí, trong khi dòng hàng vẫn giữ riêng phần xử lý tồn kho/giá vốn. |
| **Decorator** | Bổ sung cách tính/diễn giải phụ phí cho phần tính hóa đơn mà không làm thay đổi xử lý phân bổ lô hàng. |
| **Observer** | Khi dữ liệu dòng hàng, dịch vụ, khoảng cách hoặc giảm giá thay đổi, phát thông báo để tính lại phí/tổng/còn nợ. |
| **Mediator** | Điều phối giao tiếp giữa form, danh sách dòng, bộ tính hóa đơn và lưu trữ để giảm phụ thuộc trực tiếp. |

### 2\. Chiết khấu và giảm giá cấp hóa đơn

| Mẫu | Vị trí áp dụng dự kiến |
| :---- | :---- |
| **Builder** | Dựng dữ liệu đầu vào/kết quả hóa đơn từ tiền hàng, dịch vụ, giao hàng, tỷ lệ giảm và tiền đã trả. |
| **Facade** | Cung cấp một điểm gọi đơn giản để form yêu cầu tính và kiểm tra hóa đơn. |
| **Strategy** | Đóng gói quy tắc áp dụng theo lựa chọn Giảm giá/Chiết khấu; nếu sau này nghiệp vụ có thuật toán riêng thì có thể thay thế độc lập. |
| **Chain of Responsibility** | Kiểm tra tuần tự tỷ lệ giảm, khoảng cách, phí nhập tay, tổng tiền không âm và số đã trả hợp lệ. |
| **Template Method** | Chuẩn hóa chuỗi bước tính; cho phép khác biệt hợp lệ giữa bán lẻ và bán sỉ, đặc biệt ở chính sách phí vận chuyển. |

## Các giai đoạn thực hiện

1. **Chốt quy tắc tính còn mở**  
     
   - Xác nhận cơ sở tính giá trị đơn hàng để chọn bậc phí vận chuyển sỉ.  
   - Xác nhận thứ tự chiết khấu so với dịch vụ/vận chuyển, cách làm tròn và ý nghĩa riêng của hai lựa chọn Giảm giá/Chiết khấu.  
   - Chốt xử lý khoảng cách 0/âm và xác nhận cách hiểu các ranh giới đã nêu ở trên.

   

2. **Thiết kế mô hình nghiệp vụ và tính tiền**  
     
   - Tách phép tính khỏi form WinForms.  
   - Dòng dịch vụ/vận chuyển không tham gia thay đổi tồn kho, giá vốn hoặc phân bổ lô.  
   - Tính tiền hàng từ `CHI_TIET_PHIEU_BAN`; tính dịch vụ/vận chuyển và giảm giá riêng; tổng phải trả và còn nợ được tạo từ các thành phần đó.  
   - Xây dựng Factory Method, Composite, Decorator, Observer, Mediator cho các dòng phụ phí; Builder, Facade, Strategy, Chain of Responsibility, Template Method cho tính/kiểm tra hóa đơn.

   

3. **Cập nhật giao diện lập hóa đơn**  
     
   - Bổ sung lựa chọn lấy trực tiếp/giao hàng, nhập khoảng cách và phí vận chuyển nhập tay khi vượt ngưỡng bán lẻ.  
   - Bổ sung nhập phí bốc xếp, đóng gói (bán sỉ) và dòng dịch vụ tự nhập.  
   - Bổ sung combobox loại giảm giá/chiết khấu, tỷ lệ phần trăm; kiểm tra tỷ lệ nhỏ hơn 20%.  
   - Hiển thị riêng tiền hàng, tổng dịch vụ, phí vận chuyển, số tiền giảm, tổng thanh toán, đã trả và còn nợ.  
   - Vì hóa đơn cũ không cho sửa, các điều khiển nhập liệu chỉ hoạt động khi lập hóa đơn mới; hóa đơn cũ chỉ hiển thị/in.  
   - Giữ hành vi nhất quán ở bán lẻ và bán sỉ, đồng thời tránh nhân đôi phần tính toán.   
     

     
4. **Cập nhật lưu/đọc hóa đơn**  
     
   - Lưu thông tin giao hàng, khoảng cách, phí vận chuyển, loại/tỷ lệ/số tiền giảm và danh sách dịch vụ cùng phiếu.  
   - Lưu phiếu, chi tiết hàng và dịch vụ trong một transaction.  
   - Cập nhật [BusinessObject/PhieuBan.cs](http://BusinessObject/PhieuBan.cs), [Controller/PhieuBanController.cs](http://Controller/PhieuBanController.cs) và [DataLayer/BanLeRepository.cs](http://DataLayer/BanLeRepository.cs).  
   - Cập nhật truy vấn danh sách, nợ khách hàng và báo cáo theo đúng tổng phải trả; bảo đảm dữ liệu hóa đơn cũ được quy đổi/đọc tương thích.  
   - Không mở chức năng cập nhật hóa đơn cũ; thao tác xóa hiện hữu cần được rà soát riêng để không phá tính nhất quán dữ liệu.

   

5. **Cập nhật mẫu in**  
     
   - Điều chỉnh [frmInPhieuBan.cs](http://frmInPhieuBan.cs) và [Report/rptPhieuBan.rdlc](http://Report/rptPhieuBan.rdlc) để in dịch vụ, giao hàng, khoảng cách, phí vận chuyển, loại/tỷ lệ/số tiền giảm và tổng thanh toán.  
   - Kiểm tra hóa đơn cũ chưa có dữ liệu phụ phí vẫn mở/in được.

   

6. **Kiểm thử**  
     
   - Kiểm thử lấy trực tiếp, giao hàng theo mọi bậc bán lẻ/bán sỉ, mốc biên, phí nhập tay và giá trị đơn hàng xác định bậc phí.  
   - Kiểm thử dịch vụ bốc xếp/đóng gói/tự nhập, không dịch vụ và không vận chuyển.  
   - Kiểm thử tỷ lệ 0%, gần 20%, bằng 20%, âm; kiểm tra làm tròn theo quy tắc đã thống nhất.  
   - Kiểm thử tổng tiền, đã trả/còn nợ, lưu transaction, đọc/in hóa đơn mới và cũ.  
   - Xác nhận phí/giảm không ảnh hưởng tồn kho, giá vốn và phân bổ lô; xác nhận hóa đơn đã lưu không sửa được.

## File dự kiến liên quan

### Sẽ sửa

- [frmBanLe.cs](http://frmBanLe.cs), [frmBanLe.Designer.cs](http://frmBanLe.Designer.cs)  
- [frmBanSi.cs](http://frmBanSi.cs), [frmBanSi.Designer.cs](http://frmBanSi.Designer.cs)  
- [BusinessObject/PhieuBan.cs](http://BusinessObject/PhieuBan.cs)  
- [Controller/PhieuBanController.cs](http://Controller/PhieuBanController.cs)  
- [DataLayer/BanLeRepository.cs](http://DataLayer/BanLeRepository.cs)  
- [frmInPhieuBan.cs](http://frmInPhieuBan.cs)  
- [Report/rptPhieuBan.rdlc](http://Report/rptPhieuBan.rdlc)  
- [CuahangNongDuoc.csproj](http://CuahangNongDuoc.csproj) để khai báo các lớp mới (dự án .NET Framework kiểu cũ).  
- Các form/dữ liệu báo cáo danh sách hoặc công nợ khác nếu truy vấn hiện tại đang dựa trên `PHIEU_BAN.TONG_TIEN` như tiền hàng thuần.

### Có thể tạo mới

- Các lớp dòng phụ phí/dịch vụ và mô hình tính hóa đơn.  
- Các lớp áp dụng pattern cho dòng phụ phí và quy trình giảm giá/tính tổng.  
- Lớp truy cập dữ liệu dịch vụ hóa đơn, nếu lưu dịch vụ trong bảng chi tiết riêng.

## Cơ sở dữ liệu — Tôi đã sửa db theo phần dưới đây rồi cứ sử dụng các field này khi code 

Theo schema hiện tại, **không cần sửa `SAN_PHAM` hoặc `MA_SAN_PHAM`**: phí dịch vụ/vận chuyển và giảm giá thuộc hóa đơn, không thuộc sản phẩm hay lô hàng. Dự kiến sửa `PHIEU_BAN` và tạo một bảng chi tiết dịch vụ/phụ phí mới.

### 1\. Bảng `PHIEU_BAN` — thêm các cột

| Cột đề xuất | Mục đích |
| :---- | :---- |
| `TIEN_HANG` | Tổng tiền các dòng hàng; giúp phân biệt tiền hàng với tổng phải trả. |
| `TONG_DICH_VU` | Tổng phí bốc xếp, đóng gói và dịch vụ tự nhập. |
| `GIAO_HANG` | Cờ có giao hàng hay lấy trực tiếp. |
| `KHOANG_CACH_KM` | Khoảng cách nhân viên nhập khi chọn giao hàng. |
| `PHI_VAN_CHUYEN` | Phí vận chuyển đã tính/nhập tại thời điểm lập hóa đơn. |
| `LOAI_GIAM_GIA` | Giá trị lựa chọn “Giảm giá” hoặc “Chiết khấu”. |
| `TY_LE_GIAM_GIA` | Tỷ lệ phần trăm đã áp dụng (nhỏ hơn 20%). |
| `TIEN_GIAM_GIA` | Số tiền giảm thực tế sau quy tắc làm tròn. |

Giữ các cột hiện tại `TONG_TIEN`, `DA_TRA`, `CON_NO`; đề xuất quy ước `TONG_TIEN` là **tổng phải trả cuối cùng sau phụ phí và giảm giá**, `CON_NO = TONG_TIEN - DA_TRA`. Công thức dự kiến: `TONG_TIEN = TIEN_HANG + TONG_DICH_VU + PHI_VAN_CHUYEN - TIEN_GIAM_GIA` (thứ tự/cơ sở giảm cần chốt ở giai đoạn 1). Không lưu lại đơn giá phí vận chuyển theo bảng giá; lưu số phí thực tế trên phiếu để hóa đơn cũ không bị thay đổi khi chính sách giá đổi.

### 2\. Tạo bảng mới `CHI_TIET_DICH_VU_PHIEU_BAN`

Các cột đề xuất:

- `ID` — khóa chính của dòng (tự tăng hoặc mã duy nhất theo quy ước database hiện tại).  
- `ID_PHIEU_BAN` — khóa ngoại tới `PHIEU_BAN.ID`.  
- `TEN_DICH_VU` — tên hiển thị, ví dụ “Phí bốc xếp”, “Phí đóng gói” hoặc tên do nhân viên nhập.  
- `SO_TIEN` — số tiền dịch vụ.

Bảng này chỉ chứa dịch vụ phát sinh; phí vận chuyển lưu tại `PHIEU_BAN` vì đã chốt tối đa một dòng vận chuyển mỗi hóa đơn. Khi xóa phiếu, xóa các dòng dịch vụ cùng transaction hoặc thiết lập cascade phù hợp với cơ chế hiện có.

### Quy đổi dữ liệu hiện có

Sau khi thêm cột, cần gán cho hóa đơn cũ: `TIEN_HANG = TONG_TIEN` cũ, `TONG_DICH_VU = 0`, `GIAO_HANG = False`, `KHOANG_CACH_KM = 0`, `PHI_VAN_CHUYEN = 0`, `TY_LE_GIAM_GIA = 0`, `TIEN_GIAM_GIA = 0`; `LOAI_GIAM_GIA` để rỗng/giá trị mặc định. Trước khi cập nhật cần sao lưu DB và kiểm tra kiểu dữ liệu/giá trị mặc định tương ứng (Access/OleDb hay SQL Server) trong database thực tế.

**Chưa thực hiện thay đổi database.** Khi bắt đầu triển khai sẽ cung cấp câu lệnh/migration hoặc danh sách thay đổi cụ thể để bạn tự áp dụng.  
