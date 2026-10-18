using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;
using CuahangNongduoc.Controller;
using CuahangNongduoc.BusinessObject;

namespace CuahangNongduoc
{
    public partial class frmNhapHang : Form
    {
        private SanPhamController ctrlSanPham = new SanPhamController();
        private PhieuNhapController ctrl = new PhieuNhapController();
        private MaSanPhamController ctrlMaSP = new MaSanPhamController();
        private NhaCungCapController ctrlNCC = new NhaCungCapController();
        private Controll status = Controll.AddNew;
        private bool m_FormInitialized;
        private bool m_UpdatingAmounts;
        private string m_InitialInvoiceId;
        private bool m_MoTrucTiepCheDoChinhSua;
        private int m_ViTriDangSua = -1;

        public frmNhapHang()
        {
            InitializeComponent();

            // Các cột TONG_TIEN, DA_TRA, CON_NO, DON_GIA_NHAP, SO_LUONG trong Access đều là Long Integer (Int32),
            // tối đa 2.147.483.647. Designer cho nhập tới 10^13 nên tổng tiền vượt ngưỡng này sẽ lỗi tràn số lúc lưu
            // -> đặt giới hạn đúng bằng giới hạn của cơ sở dữ liệu cho mọi ô tiền.
            numTongTien.Maximum = Int32.MaxValue;
            numDaTra.Maximum = Int32.MaxValue;
            numConNo.Maximum = Int32.MaxValue;
            numThanhTien.Maximum = Int32.MaxValue;

            // Các trường tiền không được sửa tay; chỉ số tiền đã trả được nhập.
            numTongTien.ReadOnly = true;
            numConNo.ReadOnly = true;
            numThanhTien.ReadOnly = true;
            // ReadOnly vẫn cho phép bấm nút tăng/giảm của NumericUpDown; tắt tương tác tay hoàn toàn.
            numTongTien.Enabled = false;
            numConNo.Enabled = false;
            numThanhTien.Enabled = false;
            txtMaPhieu.ReadOnly = true;

            // Quy ước bước tăng của các trường tiền theo yêu cầu tester.
            numGiaNhap.Increment = 10000M;
            numThanhTien.Increment = 10000M;
            numTongTien.Increment = 10000M;
            numDaTra.Increment = 10000M;
            numConNo.Increment = 10000M;
            numSoLuong.Increment = 1M;

            numGiaNhap.Minimum = 0M;
            numSoLuong.Minimum = 0M;
            numDaTra.Minimum = 0M;
            numTongTien.Minimum = 0M;
            numConNo.Minimum = 0M;
            numThanhTien.Minimum = 0M;

            dataGridView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView.MultiSelect = false;
            dataGridView.AllowUserToAddRows = false;
            dataGridView.AllowUserToDeleteRows = false;
            dataGridView.ReadOnly = true;

            // Cập nhật tổng tiền xem trước khi người dùng nhập/sửa dữ liệu của dòng hiện tại.
            txtMaSo.TextChanged += new EventHandler(DongBoTongTienKhiNhap);
            txtSoLo.TextChanged += new EventHandler(DongBoTongTienKhiNhap);
            cmbSanPham.SelectedValueChanged += new EventHandler(DongBoTongTienKhiNhap);
        }

        public frmNhapHang(PhieuNhapController ctrlPN)
            : this()
        {
            if (ctrlPN != null)
            {
                // Lưu mã phiếu đang chọn trước khi nạp lại danh sách bằng Controller mới.
                m_InitialInvoiceId = ctrlPN.LayMaPhieuHienTai();
                ctrl = ctrlPN;
                status = Controll.Normal;
            }
        }

        // Dùng cho nút "Chỉnh sửa" ở danh sách phiếu nhập: mở form và vào chế độ chỉnh sửa ngay.
        public frmNhapHang(PhieuNhapController ctrlPN, bool moTrucTiepCheDoChinhSua)
            : this(ctrlPN)
        {
            m_MoTrucTiepCheDoChinhSua = moTrucTiepCheDoChinhSua;
        }

        private void frmNhapHang_Load(object sender, EventArgs e)
        {
            // Nút Xem lại cũng dùng handler Load. Tránh khởi tạo/bind form lặp lại vô tình.
            if (m_FormInitialized)
            {
                if (status == Controll.Normal)
                    NapLaiPhieuDangChon();
                else
                    MessageBox.Show("Hãy lưu hoặc thoát bản nháp trước khi tải lại dữ liệu.", "Nhập hàng",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            try
            {
                
                ctrlSanPham.HienthiAutoComboBox(cmbSanPham);
                ctrlSanPham.HienthiDataGridViewComboBoxColumn(colSanPham);
                ctrlNCC.HienthiAutoComboBox(cmbNhaCungCap);

                m_FormInitialized = true;
                if (status == Controll.AddNew)
                {
                    BatDauPhieuMoi();
                }
                else
                {
                    NapDanhSachPhieuNhap(m_InitialInvoiceId);
                    if (m_MoTrucTiepCheDoChinhSua && status == Controll.Normal)
                        toolChinhsua_Click(this, EventArgs.Empty);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Không thể khởi tạo form nhập hàng: " + ex.Message, "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                Close();
            }
        }

        private string TaoMaPhieuNhapMoi()
        {
            long ma = ThamSo.LayMaPhieuNhap();
            if (ma < 1)
                ma = 1;
            if (ma > Int32.MaxValue)
                throw new InvalidOperationException("Bộ cấp mã phiếu nhập đã vượt giới hạn số nguyên của cơ sở dữ liệu.");

            PhieuNhapController kiemTra = new PhieuNhapController();
            bool daDieuChinh = false;
            while (kiemTra.LayPhieuNhap(ma.ToString()) != null)
            {
                if (ma >= Int32.MaxValue)
                    throw new InvalidOperationException("Không thể cấp mã phiếu nhập mới vì đã đạt giới hạn của cơ sở dữ liệu.");
                ma++;
                daDieuChinh = true;
            }

            // Chỉ ghi lại bộ đếm khi phải tránh một mã đã tồn tại, hạn chế cập nhật thừa.
            if (daDieuChinh)
                ThamSo.GanMaPhieuNhap(ma);
            return ma.ToString();
        }

        private void XoaBindingThongTinPhieu()
        {
            txtMaPhieu.DataBindings.Clear();
            cmbNhaCungCap.DataBindings.Clear();
            dtNgayNhap.DataBindings.Clear();
            numTongTien.DataBindings.Clear();
            numDaTra.DataBindings.Clear();
            numConNo.DataBindings.Clear();
        }

        private void BatDauPhieuMoi()
        {
            if (bindingNavigator.BindingSource != null)
                bindingNavigator.BindingSource.CurrentChanged -= BindingSource_CurrentChanged;
            status = Controll.AddNew;
            ctrl = new PhieuNhapController();       // Bảng PHIEU_NHAP chỉ nạp schema, không bind vào phiếu cũ.
            ctrlMaSP = new MaSanPhamController();   // Bảng MA_SAN_PHAM chỉ quản lý chi tiết của bản nháp.
            XoaBindingThongTinPhieu();

            bindingNavigator.BindingSource = new BindingSource();
            DatTrangThaiNutDieuHuong(false);

            // Xóa toàn bộ dữ liệu nhập dở từ phiếu trước để không cộng nhầm tiền hoặc dùng lại số lô.
            txtMaSo.Clear();
            txtSoLo.Clear();
            if (cmbSanPham.Items.Count > 0)
                cmbSanPham.SelectedIndex = -1;

            txtMaPhieu.Text = TaoMaPhieuNhapMoi();
            dtNgayNhap.Value = DateTime.Today;
            dtNgaySanXuat.Value = DateTime.Today;
            dtNgayHetHan.Value = DateTime.Today.AddMonths(1);
            if (cmbNhaCungCap.Items.Count > 0)
                cmbNhaCungCap.SelectedIndex = -1;
            numTongTien.Value = 0M;
            numDaTra.Value = 0M;
            numConNo.Value = 0M;
            numGiaNhap.Value = 0M;
            numSoLuong.Value = 0M;
            numThanhTien.Value = 0M;

            ctrlMaSP.HienThiChiTietPhieuNhap(txtMaPhieu.Text, dataGridView);
            Allow(true);
            RecalculateConNo();
        }

        private void NapDanhSachPhieuNhap(string maPhieuCanChon)
        {
            status = Controll.Normal;
            XoaBindingThongTinPhieu();
            ctrl = new PhieuNhapController();
            ctrlMaSP = new MaSanPhamController();
            ctrl.HienthiPhieuNhap(bindingNavigator, txtMaPhieu, cmbNhaCungCap, dtNgayNhap,
                numTongTien, numDaTra, numConNo);
            GanSuKienBindingSource();
            DatTrangThaiNutDieuHuong(true);

            if (bindingNavigator.BindingSource == null || bindingNavigator.BindingSource.Count == 0)
            {
                BatDauPhieuMoi();
                return;
            }

            if (!String.IsNullOrEmpty(maPhieuCanChon))
                ctrl.ChonPhieuNhap(maPhieuCanChon);

            ctrlMaSP.HienThiChiTietPhieuNhap(txtMaPhieu.Text, dataGridView);
            Allow(false);
        }

        private void GanSuKienBindingSource()
        {
            if (bindingNavigator.BindingSource != null)
            {
                bindingNavigator.BindingSource.CurrentChanged -= BindingSource_CurrentChanged;
                bindingNavigator.BindingSource.CurrentChanged += BindingSource_CurrentChanged;
            }
        }

        private void BindingSource_CurrentChanged(object sender, EventArgs e)
        {
            BindingSource bs = sender as BindingSource;
            if (bs == null)
                return;

            if (status == Controll.Edit)
            {
                // Đang sửa: nút điều hướng có thể tự bật lại khi dữ liệu đổi, nên kéo về đúng phiếu đang sửa
                // (nếu không phần đầu phiếu hiện phiếu mới còn lưới vẫn là lô của phiếu cũ).
                if (m_ViTriDangSua >= 0 && m_ViTriDangSua < bs.Count && bs.Position != m_ViTriDangSua)
                    bs.Position = m_ViTriDangSua;
                return;
            }

            if (status == Controll.Normal)
            {
                // Lấy mã từ dòng hiện tại của BindingSource, không đọc txtMaPhieu (có thể chưa kịp cập nhật).
                DataRowView drv = bs.Current as DataRowView;
                string maPhieu = (drv != null) ? Convert.ToString(drv["ID"]) : txtMaPhieu.Text;
                if (!String.IsNullOrEmpty(maPhieu))
                {
                    ctrlMaSP.HienThiChiTietPhieuNhap(maPhieu, dataGridView);
                    RecalculateConNo();
                }
            }
        }

        private void NapLaiPhieuDangChon()
        {
            string maPhieu = txtMaPhieu.Text.Trim();
            NapDanhSachPhieuNhap(maPhieu);
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (status == Controll.Normal)
            {
                MessageBox.Show("Nhấn Chỉnh sửa trước khi thay đổi phiếu đã lưu.", "Nhập hàng",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            string maLo = txtMaSo.Text.Trim();
            string soLo = txtSoLo.Text.Trim();
            string idSanPham = Convert.ToString(cmbSanPham.SelectedValue).Trim();

            // Guard clauses: mỗi lỗi đều dừng luồng xử lý ngay tại đây.
            if (String.IsNullOrEmpty(maLo))
            {
                MessageBox.Show("Vui lòng nhập Mã số của lô hàng.", "Nhập hàng", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtMaSo.Focus();
                return;
            }
            if (String.IsNullOrEmpty(soLo))
            {
                MessageBox.Show("Vui lòng nhập Số lô.", "Nhập hàng", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtSoLo.Focus();
                return;
            }
            if (cmbSanPham.SelectedValue == null || cmbSanPham.SelectedValue == DBNull.Value || String.IsNullOrEmpty(idSanPham))
            {
                MessageBox.Show("Vui lòng chọn sản phẩm.", "Nhập hàng", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cmbSanPham.Focus();
                return;
            }
            if (numGiaNhap.Value <= 0M)
            {
                MessageBox.Show("Đơn giá nhập phải lớn hơn 0.", "Nhập hàng", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                numGiaNhap.Focus();
                return;
            }
            if (numSoLuong.Value <= 0M)
            {
                MessageBox.Show("Số lượng nhập phải lớn hơn 0.", "Nhập hàng", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                numSoLuong.Focus();
                return;
            }
            if (dtNgaySanXuat.Value.Date > dtNgayNhap.Value.Date)
            {
                MessageBox.Show("Ngày sản xuất không được sau ngày nhập.", "Nhập hàng", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                dtNgaySanXuat.Focus();
                return;
            }
            if (dtNgayHetHan.Value.Date <= dtNgaySanXuat.Value.Date)
            {
                MessageBox.Show("Ngày hết hạn phải sau ngày sản xuất.", "Nhập hàng", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                dtNgayHetHan.Focus();
                return;
            }
            if (dtNgayHetHan.Value.Date <= dtNgayNhap.Value.Date)
            {
                MessageBox.Show("Ngày hết hạn phải sau ngày nhập.", "Nhập hàng", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                dtNgayHetHan.Focus();
                return;
            }
            if (dtNgayHetHan.Value.Date <= DateTime.Today)
            {
                MessageBox.Show("Ngày hết hạn phải sau ngày hiện tại.", "Nhập hàng", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                dtNgayHetHan.Focus();
                return;
            }

            decimal thanhTien = numGiaNhap.Value * numSoLuong.Value;
            if (thanhTien <= 0M || thanhTien > numThanhTien.Maximum)
            {
                MessageBox.Show("Thành tiền của lô hàng không hợp lệ hoặc vượt giới hạn cho phép.", "Nhập hàng", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            decimal tongChiTietHienCo = TinhTongTienCacLo(null);
            if (tongChiTietHienCo + thanhTien > numTongTien.Maximum)
            {
                MessageBox.Show("Tổng tiền phiếu nhập vượt giới hạn cho phép.", "Nhập hàng", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            foreach (DataGridViewRow view in dataGridView.Rows)
            {
                if (view.IsNewRow || view.Cells["colMaSanPham"].Value == null)
                    continue;
                if (String.Equals(maLo, Convert.ToString(view.Cells["colMaSanPham"].Value), StringComparison.OrdinalIgnoreCase))
                {
                    MessageBox.Show("Mã số lô đã có trong phiếu này. Vui lòng nhập mã khác.", "Nhập hàng", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtMaSo.Focus();
                    return;
                }
            }

            try
            {
                if (new MaSanPhamController().LayMaSanPham(maLo) != null)
                {
                    MessageBox.Show("Mã số lô đã tồn tại trong cơ sở dữ liệu. Vui lòng nhập mã khác.", "Nhập hàng", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtMaSo.Focus();
                    return;
                }

                ctrl.KiemTraHopLe(Convert.ToInt64(numSoLuong.Value), Convert.ToInt64(numGiaNhap.Value));
                DataRow row = ctrlMaSP.NewRow();
                row["ID_SAN_PHAM"] = idSanPham;
                row["ID_PHIEU_NHAP"] = txtMaPhieu.Text.Trim();
                row["ID"] = maLo;
                row["DON_GIA_NHAP"] = Convert.ToInt64(numGiaNhap.Value);
                row["SO_LUONG"] = Convert.ToInt64(numSoLuong.Value);
                row["NGAY_NHAP"] = dtNgayNhap.Value.Date;
                row["NGAY_SAN_XUAT"] = dtNgaySanXuat.Value.Date;
                row["NGAY_HET_HAN"] = dtNgayHetHan.Value.Date;
                row["SO_LO"] = soLo;

                ctrlMaSP.Add(row);

                // Dòng đã vào bảng chi tiết nên xóa dữ liệu đang nhập trước khi tính lại tổng.
                txtMaSo.Clear();
                txtSoLo.Clear();
                numGiaNhap.Value = 0M;
                numSoLuong.Value = 0M;
                numThanhTien.Value = 0M;
                CapNhatTongTienTuChiTiet(false);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Không thể thêm lô hàng: " + ex.Message, "Lỗi dữ liệu", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void numGiaNhap_ValueChanged(object sender, EventArgs e)
        {
            decimal thanhTien = numGiaNhap.Value * numSoLuong.Value;
            numThanhTien.Value = thanhTien > numThanhTien.Maximum ? numThanhTien.Maximum : thanhTien;
            if (m_FormInitialized)
                CapNhatTongTienTuChiTiet(true);
        }

        private void DongBoTongTienKhiNhap(object sender, EventArgs e)
        {
            if (m_FormInitialized)
                CapNhatTongTienTuChiTiet(true);
        }

        // Tổng tiền gốc đã lưu của phiếu đang sửa (0 nếu là phiếu mới).
        private decimal TongTienGocCuaPhieu()
        {
            if (status != Controll.Edit || bindingNavigator.BindingSource == null)
                return 0M;
            DataRowView drv = bindingNavigator.BindingSource.Current as DataRowView;
            if (drv == null || drv.Row == null || drv.Row.RowState == DataRowState.Deleted ||
                drv.Row.RowState == DataRowState.Detached)
                return 0M;
            return Convert.ToDecimal(drv.Row["TONG_TIEN", DataRowVersion.Original]);
        }

        private static decimal TienCuaLo(DataRow row, DataRowVersion version)
        {
            return Convert.ToDecimal(row["DON_GIA_NHAP", version]) * Convert.ToDecimal(row["SO_LUONG", version]);
        }

        // Tổng tiền phiếu = tổng tiền đã lưu + các lô mới thêm - các lô vừa xóa.
        // KHÔNG cộng lại từ SO_LUONG của lô cũ vì SO_LUONG là số còn lại sau khi bán:
        // làm vậy sẽ hạ Tổng tiền/Còn nợ của phiếu đã có lô bán (vd phiếu 300.000 thành 180.000).
        // boQuaDong: dòng sắp bị xóa (nếu là lô mới thì bỏ qua, nếu là lô cũ thì trừ ra).
        private decimal TinhTongTienCacLo(DataRow boQuaDong)
        {
            decimal tong = TongTienGocCuaPhieu();
            DataService bangChiTiet = ctrlMaSP.GetDataService();
            foreach (DataRow row in bangChiTiet.Rows)
            {
                if (row.RowState == DataRowState.Detached)
                    continue;
                bool laDongBoQua = Object.ReferenceEquals(row, boQuaDong);

                if (row.RowState == DataRowState.Deleted)
                {
                    tong -= TienCuaLo(row, DataRowVersion.Original);
                }
                else if (row.RowState == DataRowState.Added)
                {
                    if (laDongBoQua)
                        continue;
                    decimal donGia = Convert.ToDecimal(row["DON_GIA_NHAP"]);
                    decimal soLuong = Convert.ToDecimal(row["SO_LUONG"]);
                    if (donGia < 0M || soLuong < 0M)
                        throw new InvalidOperationException("Đơn giá và số lượng của lô không được âm.");
                    tong += donGia * soLuong;
                }
                else if (laDongBoQua)
                {
                    tong -= TienCuaLo(row, DataRowVersion.Current);
                }
            }
            return tong < 0M ? 0M : tong;
        }

        private bool DongNhapTamHopLe()
        {
            return status != Controll.Normal &&
                cmbSanPham.SelectedValue != null && cmbSanPham.SelectedValue != DBNull.Value &&
                !String.IsNullOrEmpty(Convert.ToString(cmbSanPham.SelectedValue).Trim()) &&
                !String.IsNullOrEmpty(txtMaSo.Text.Trim()) &&
                !String.IsNullOrEmpty(txtSoLo.Text.Trim()) &&
                numGiaNhap.Value > 0M && numSoLuong.Value > 0M;
        }

        private void CapNhatTongTienTuChiTiet(bool baoGomDongDangNhap)
        {
            // Chế độ xem: Tổng tiền là số đã lưu, không tính lại (trước đây bấm "Thêm sản phẩm" làm Tổng tiền về 0).
            if (status == Controll.Normal)
                return;

            decimal tong = TinhTongTienCacLo(null);
            if (baoGomDongDangNhap && DongNhapTamHopLe())
                tong += numGiaNhap.Value * numSoLuong.Value;

            // Ô nhập có giới hạn theo thiết kế Access; thao tác Lưu/Thêm sẽ từ chối nếu vượt giới hạn.
            if (tong > numTongTien.Maximum)
                tong = numTongTien.Maximum;
            if (tong < numTongTien.Minimum)
                tong = numTongTien.Minimum;

            numTongTien.Value = tong;
            RecalculateConNo();
        }

        private void toolLuuThoat_Click(object sender, EventArgs e)
        {
            try
            {
                Luu();
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Không thể lưu phiếu nhập: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CommitHeaderBindings()
        {
            // Đẩy mọi giá trị đã thay đổi từ control xuống DataRow trước khi DAO lưu.
            Control[] controls = new Control[] { txtMaPhieu, cmbNhaCungCap, dtNgayNhap, numTongTien, numDaTra, numConNo };
            foreach (Control control in controls)
            {
                foreach (Binding binding in control.DataBindings)
                    binding.WriteValue();
            }
        }

        // Ngày nhập của phiếu đã bị đổi so với bản đã lưu? (phiếu mới: luôn coi là đổi để đồng bộ cho các lô)
        private bool NgayNhapDaDoi()
        {
            if (status != Controll.Edit)
                return true;
            DataRowView drv = (bindingNavigator.BindingSource == null) ? null : bindingNavigator.BindingSource.Current as DataRowView;
            if (drv == null || drv.Row == null || drv.Row.RowState == DataRowState.Deleted ||
                drv.Row.RowState == DataRowState.Detached)
                return false;
            DateTime goc = Convert.ToDateTime(drv.Row["NGAY_NHAP", DataRowVersion.Original]).Date;
            return goc != dtNgayNhap.Value.Date;
        }

        private void KiemTraPhieuNhap()
        {
            if (String.IsNullOrEmpty(txtMaPhieu.Text.Trim()))
                throw new InvalidOperationException("Mã phiếu nhập chưa được cấp.");
            if (cmbNhaCungCap.SelectedValue == null || cmbNhaCungCap.SelectedValue == DBNull.Value ||
                String.IsNullOrEmpty(Convert.ToString(cmbNhaCungCap.SelectedValue).Trim()))
                throw new InvalidOperationException("Vui lòng chọn nhà cung cấp.");

            bool ngayNhapDoi = NgayNhapDaDoi();
            int soChiTiet = 0;
            foreach (DataGridViewRow view in dataGridView.Rows)
            {
                if (view.IsNewRow)
                    continue;
                DataRowView drv = view.DataBoundItem as DataRowView;
                if (drv == null || drv.Row == null || drv.Row.RowState == DataRowState.Deleted ||
                    drv.Row.RowState == DataRowState.Detached)
                    continue;

                string id = Convert.ToString(drv["ID"]).Trim();
                bool loMoi = drv.Row.RowState == DataRowState.Added;
                if (String.IsNullOrEmpty(id))
                    throw new InvalidOperationException("Mã lô không được để trống.");
                soChiTiet++;

                if (loMoi)
                {
                    string soLo = Convert.ToString(drv["SO_LO"]).Trim();
                    if (String.IsNullOrEmpty(soLo))
                        throw new InvalidOperationException("Số lô của lô mới không được để trống.");
                    // Chỉ lô mới mới bắt buộc SL > 0; lô cũ có thể đã bán hết (SL còn lại = 0).
                    if (Convert.ToDecimal(drv["DON_GIA_NHAP"]) <= 0M || Convert.ToDecimal(drv["SO_LUONG"]) <= 0M)
                        throw new InvalidOperationException("Đơn giá và số lượng của mỗi lô mới phải lớn hơn 0.");
                }

                // Lô cũ chỉ kiểm tra ngày khi người dùng đổi ngày nhập của phiếu.
                if (loMoi || ngayNhapDoi)
                {
                    DateTime ngaySanXuat = Convert.ToDateTime(drv["NGAY_SAN_XUAT"]).Date;
                    DateTime ngayHetHan = Convert.ToDateTime(drv["NGAY_HET_HAN"]).Date;
                    if (ngaySanXuat > dtNgayNhap.Value.Date)
                        throw new InvalidOperationException("Ngày sản xuất của lô " + id + " không được sau ngày nhập.");
                    if (ngayHetHan <= dtNgayNhap.Value.Date)
                        throw new InvalidOperationException("Ngày hết hạn của lô " + id + " phải sau ngày nhập.");
                }
            }

            if (soChiTiet == 0)
                throw new InvalidOperationException("Phiếu nhập phải có ít nhất một lô hàng.");

            decimal tongTien = TinhTongTienCacLo(null);
            if (tongTien > numTongTien.Maximum)
                throw new InvalidOperationException("Tổng tiền phiếu nhập vượt giới hạn của cơ sở dữ liệu.");

            // Không tự giảm tiền đã trả khi xóa/sửa chi tiết làm tổng tiền giảm.
            if (numDaTra.Value > tongTien)
                throw new InvalidOperationException("Số tiền đã trả lớn hơn tổng tiền của phiếu. Hãy điều chỉnh số đã trả trước khi lưu.");

            numTongTien.Value = tongTien;
            if (numTongTien.Value < 0M || numDaTra.Value < 0M)
                throw new InvalidOperationException("Tổng tiền và số tiền đã trả không được âm.");
            RecalculateConNo();
        }

        private void Luu()
        {
            if (status == Controll.Normal)
                return;

            ValidateChildren();
            // Tính/kiểm tra tổng tiền trước, sau đó mới đẩy kết quả cuối cùng về DataRow.
            // Nếu ghi binding trước, tổng tiền/còn nợ vừa tính lại có thể không được lưu.
            KiemTraPhieuNhap();
            CommitHeaderBindings();
            if (bindingNavigator.BindingSource != null && bindingNavigator.BindingSource.Current is DataRowView)
                ((DataRowView)bindingNavigator.BindingSource.Current).EndEdit();

            DongBoNgayNhapChiTiet();
            string maPhieu = txtMaPhieu.Text.Trim();
            if (status == Controll.AddNew)
                ThemMoi();
            else if (status == Controll.Edit)
                CapNhat();
            else
                throw new InvalidOperationException("Trạng thái phiếu nhập không hợp lệ.");

            // Chỉ chuyển về Normal sau khi transaction lưu dữ liệu đã commit thành công.
            NapDanhSachPhieuNhap(maPhieu);
        }

        private void DongBoNgayNhapChiTiet()
        {
            // Ngày nhập trên phiếu và trên từng lô phải nhất quán khi ngày phiếu được chỉnh sửa.
            // Không ghi lại các lô cũ nếu ngày phiếu không đổi (tránh UPDATE vô ích lên mọi lô).
            if (!NgayNhapDaDoi())
                return;
            foreach (DataGridViewRow view in dataGridView.Rows)
            {
                if (view.IsNewRow)
                    continue;
                DataRowView row = view.DataBoundItem as DataRowView;
                if (row == null || row.Row == null || row.Row.RowState == DataRowState.Deleted ||
                    row.Row.RowState == DataRowState.Detached)
                    continue;
                row["NGAY_NHAP"] = dtNgayNhap.Value.Date;
            }
        }

        private List<string> LayDanhSachSanPhamCanTinhLai()
        {
            List<string> ids = new List<string>();
            DataTable loMoi = ctrlMaSP.LayCacLoMoi();
            DataTable loDaXoa = ctrlMaSP.LayCacLoDaXoa();
            foreach (DataRow row in loMoi.Rows)
            {
                string id = Convert.ToString(row["ID_SAN_PHAM"]);
                if (!String.IsNullOrEmpty(id) && !ids.Contains(id))
                    ids.Add(id);
            }
            foreach (DataRow row in loDaXoa.Rows)
            {
                string id = Convert.ToString(row["ID_SAN_PHAM"]);
                if (!String.IsNullOrEmpty(id) && !ids.Contains(id))
                    ids.Add(id);
            }
            return ids;
        }

        private void CapNhat()
        {
            //DataService.ExecuteDataTableTransaction(LayDanhSachSanPhamCanTinhLai(),
            //    ctrl.GetDataService(), ctrlMaSP.GetDataService());
        }

        private void ThemMoi()
        {
            string maPhieu = txtMaPhieu.Text.Trim();
            if (new PhieuNhapController().LayPhieuNhap(maPhieu) != null)
                throw new InvalidOperationException("Mã phiếu nhập đã tồn tại. Vui lòng tạo mã mới.");

            DataTable cacLoMoi = ctrlMaSP.LayCacLoMoi();
            if (cacLoMoi.Rows.Count == 0)
                throw new InvalidOperationException("Không thể lưu phiếu nhập không có lô hàng.");

            List<string> sanPhamCanTinhLai = new List<string>();
            foreach (DataRow lo in cacLoMoi.Rows)
            {
                string idSanPham = Convert.ToString(lo["ID_SAN_PHAM"]);
                if (!String.IsNullOrEmpty(idSanPham) && !sanPhamCanTinhLai.Contains(idSanPham))
                    sanPhamCanTinhLai.Add(idSanPham);
            }

            // Nếu lần lưu trước thất bại, DataTable vẫn giữ dòng Added trong bộ nhớ.
            // Tái sử dụng dòng nháp đó thay vì thêm bản ghi trùng khi người dùng thử lưu lại.
            DataRow row = null;
            DataService bangPhieu = ctrl.GetDataService();
            foreach (DataRow dongHienCo in bangPhieu.Rows)
            {
                if (dongHienCo.RowState == DataRowState.Deleted || dongHienCo.RowState == DataRowState.Detached)
                    continue;
                if (String.Equals(Convert.ToString(dongHienCo["ID"]), maPhieu, StringComparison.OrdinalIgnoreCase))
                {
                    row = dongHienCo;
                    break;
                }
            }
            if (row == null)
            {
                row = ctrl.NewRow();
                ctrl.Add(row);
            }
            row["ID"] = maPhieu;
            row["NGAY_NHAP"] = dtNgayNhap.Value.Date;
            row["TONG_TIEN"] = Convert.ToDecimal(numTongTien.Value);
            row["ID_NHA_CUNG_CAP"] = cmbNhaCungCap.SelectedValue;
            row["DA_TRA"] = Convert.ToDecimal(numDaTra.Value);
            row["CON_NO"] = Convert.ToDecimal(numConNo.Value);

            int affected = DataService.ExecuteDataTableTransaction(sanPhamCanTinhLai,
                ctrl.GetDataService(), ctrlMaSP.GetDataService());
            if (affected <= 0)
                throw new InvalidOperationException("Không có dữ liệu nào được lưu. Vui lòng kiểm tra lại phiếu nhập.");

            // Bộ cấp mã sẽ tự dò mã đã tồn tại ở lần tạo phiếu tiếp theo;
            // không cập nhật bộ đếm ở ngoài transaction vừa lưu phiếu để tránh báo lỗi giả sau khi đã commit.
        }

        private void toolLuuThem_Click(object sender, EventArgs e)
        {
            try
            {
                Luu();
                BatDauPhieuMoi();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Không thể lưu phiếu nhập: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void toolSavePrint_Click(object sender, EventArgs e)
        {
            try
            {
                if (status != Controll.Normal)
                    Luu();

                string maPhieu = txtMaPhieu.Text.Trim();
                PhieuNhap phieu = new PhieuNhapController().LayPhieuNhap(maPhieu);
                if (phieu == null)
                {
                    MessageBox.Show("Không tìm thấy phiếu nhập để in. Hãy lưu phiếu trước.", "In phiếu nhập",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                frmInPhieuNhap formIn = new frmInPhieuNhap(phieu);
                formIn.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Không thể lưu hoặc mở trang in: " + ex.Message, "Lỗi in phiếu nhập",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void toolThoat_Click(object sender, EventArgs e)
        {
            if (status != Controll.Normal)
            {
                DialogResult result = MessageBox.Show("Bạn có muốn lưu phiếu nhập trước khi thoát không?", "Nhập hàng",
                    MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                if (result == DialogResult.Cancel)
                    return;
                if (result == DialogResult.Yes)
                {
                    try { Luu(); }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Không thể lưu phiếu nhập: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }
            }
            Close();
        }

        private void btnThemSanPham_Click(object sender, EventArgs e)
        {
            using (frmSanPham formSanPham = new frmSanPham())
                formSanPham.ShowDialog();
            ctrlSanPham.HienthiAutoComboBox(cmbSanPham);
            ctrlSanPham.HienthiDataGridViewComboBoxColumn(colSanPham);
        }

        private void dataGridView_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            e.ThrowException = false;
            e.Cancel = true;
        }

        private void Allow(bool val)
        {
            txtMaPhieu.ReadOnly = true;
            txtMaPhieu.Enabled = true;
            cmbNhaCungCap.Enabled = val;
            dtNgayNhap.Enabled = val;
            numTongTien.ReadOnly = true;
            numTongTien.Enabled = false;
            numConNo.ReadOnly = true;
            numConNo.Enabled = false;
            numDaTra.Enabled = val;
            cmbSanPham.Enabled = val;
            txtMaSo.Enabled = val;
            txtSoLo.Enabled = val;
            numGiaNhap.Enabled = val;
            numSoLuong.Enabled = val;
            dtNgaySanXuat.Enabled = val;
            dtNgayHetHan.Enabled = val;
            numThanhTien.ReadOnly = true;
            numThanhTien.Enabled = false;
            btnAdd.Enabled = val;
            btnRemove.Enabled = val;
            dataGridView.Enabled = true;
            toolChinhsua.Enabled = status == Controll.Normal;
            toolXoa.Enabled = status == Controll.Normal;
        }

        private void DatTrangThaiNutDieuHuong(bool enabled)
        {
            bindingNavigatorMoveFirstItem.Enabled = enabled;
            bindingNavigatorMovePreviousItem.Enabled = enabled;
            bindingNavigatorMoveNextItem.Enabled = enabled;
            bindingNavigatorMoveLastItem.Enabled = enabled;
            bindingNavigatorPositionItem.Enabled = enabled;
        }

        private void toolChinhsua_Click(object sender, EventArgs e)
        {
            if (status != Controll.Normal)
                return;
            BindingSource bs = bindingNavigator.BindingSource;
            if (bs == null || bs.Current == null)
            {
                MessageBox.Show("Không có phiếu nhập nào để chỉnh sửa.", "Nhập hàng",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            m_ViTriDangSua = bs.Position;
            status = Controll.Edit;
            Allow(true);
            DatTrangThaiNutDieuHuong(false);
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            if (status == Controll.Normal)
            {
                MessageBox.Show("Nhấn Chỉnh sửa trước khi xóa lô hàng của phiếu đã lưu.", "Nhập hàng",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (dataGridView.CurrentRow == null || dataGridView.CurrentRow.IsNewRow)
            {
                MessageBox.Show("Vui lòng chọn lô hàng cần xóa.", "Xóa lô hàng", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            DataRowView row = dataGridView.CurrentRow.DataBoundItem as DataRowView;
            if (row == null)
            {
                MessageBox.Show("Không đọc được dòng lô hàng đang chọn.", "Xóa lô hàng", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (MessageBox.Show("Bạn chắc chắn muốn xóa lô hàng đang chọn khỏi phiếu nhập?", "Xóa lô hàng",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                return;

            try
            {
                if (row.Row.RowState != DataRowState.Added && ctrlMaSP.CoLoDaBan(Convert.ToString(row["ID"])))
                {
                    MessageBox.Show("Không thể xóa lô này vì đã được sử dụng trong hóa đơn bán.", "Không thể xóa",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                decimal tongSauKhiXoa = TinhTongTienCacLo(row.Row);
                int detailCount = 0;
                foreach (DataGridViewRow item in dataGridView.Rows)
                {
                    if (item.IsNewRow || item.DataBoundItem == null || Object.ReferenceEquals(item, dataGridView.CurrentRow))
                        continue;
                    DataRowView other = item.DataBoundItem as DataRowView;
                    if (other != null && other.Row.RowState != DataRowState.Deleted && other.Row.RowState != DataRowState.Detached)
                        detailCount++;
                }
                if (detailCount == 0 && status == Controll.Edit)
                {
                    MessageBox.Show("Phiếu nhập đã lưu phải còn ít nhất một lô hàng.", "Không thể xóa",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (numDaTra.Value > tongSauKhiXoa)
                {
                    MessageBox.Show("Không thể xóa lô này vì số tiền đã trả sẽ lớn hơn tổng tiền còn lại. Hãy điều chỉnh số đã trả trước.",
                        "Không thể xóa", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                row.Delete();
                CapNhatTongTienTuChiTiet(true);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Không thể xóa lô hàng: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dataGridView_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e)
        {
            // Xóa dòng chỉ được thực hiện qua nút btnRemove_Click để bảo đảm kiểm tra lô đã bán.
            e.Cancel = true;
        }

        private void toolXoa_Click(object sender, EventArgs e)
        {
            if (status != Controll.Normal)
            {
                MessageBox.Show("Hãy lưu hoặc thoát chế độ chỉnh sửa trước khi xóa toàn bộ phiếu nhập.",
                    "Nhập hàng", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            string maPhieu = txtMaPhieu.Text.Trim();
            if (String.IsNullOrEmpty(maPhieu))
            {
                MessageBox.Show("Không xác định được mã phiếu nhập.", "Xóa phiếu", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (MessageBox.Show("Bạn chắc chắn muốn xóa toàn bộ phiếu nhập " + maPhieu + "?", "Xóa phiếu nhập",
                MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
                return;

            try
            {
                if (ctrlMaSP.CoLoDaBanTrongPhieuNhap(maPhieu))
                {
                    MessageBox.Show("Không thể xóa phiếu vì có lô đã được sử dụng trong hóa đơn bán.", "Không thể xóa",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                DataTable sanPham = ctrlMaSP.DanhSachSanPhamTheoPhieuNhap(maPhieu);
                List<string> idsSanPham = new List<string>();
                foreach (DataRow row in sanPham.Rows)
                {
                    string id = Convert.ToString(row["ID_SAN_PHAM"]);
                    if (!String.IsNullOrEmpty(id) && !idsSanPham.Contains(id))
                        idsSanPham.Add(id);
                }

                int affected = ctrl.XoaPhieuNhapVaCacLo(maPhieu, idsSanPham);
                if (affected <= 0)
                    throw new InvalidOperationException("Không có phiếu nhập nào được xóa.");

                MessageBox.Show("Đã xóa phiếu nhập và đồng bộ tồn kho.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                NapDanhSachPhieuNhap(null);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Không thể xóa phiếu nhập: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void numDaTra_ValueChanged(object sender, EventArgs e)
        {
            if (m_UpdatingAmounts)
                return;

            bool paidExceedsTotal = Object.ReferenceEquals(sender, numDaTra) && numDaTra.Value > numTongTien.Value;
            if (paidExceedsTotal)
            {
                m_UpdatingAmounts = true;
                try { numDaTra.Value = numTongTien.Value; }
                finally { m_UpdatingAmounts = false; }
                if (m_FormInitialized)
                    MessageBox.Show("Số tiền đã trả không được lớn hơn tổng tiền phiếu nhập.", "Kiểm tra số liệu",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            RecalculateConNo();
        }

        private void RecalculateConNo()
        {
            if (m_UpdatingAmounts)
                return;
            m_UpdatingAmounts = true;
            try
            {
                decimal conNo = numTongTien.Value - numDaTra.Value;
                // Khi người dùng đang sửa dòng nhập tạm, không âm thầm thay đổi số tiền đã trả.
                if (conNo < numConNo.Minimum)
                    conNo = numConNo.Minimum;
                if (conNo > numConNo.Maximum)
                    conNo = numConNo.Maximum;
                numConNo.Value = conNo;
            }
            finally
            {
                m_UpdatingAmounts = false;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            using (frmNhaCungCap formNCC = new frmNhaCungCap())
                formNCC.ShowDialog();
            ctrlNCC.HienthiAutoComboBox(cmbNhaCungCap);
        }

    }
}
