using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;
using CuahangNongduoc.BusinessObject;
using CuahangNongduoc.Controller;
using CuahangNongduoc.DataLayer;

namespace CuahangNongduoc
{
    public partial class frmBanSi : Form
    {
        private readonly SanPhamController ctrlSanPham = new SanPhamController();
        private readonly KhachHangController ctrlKhachHang = new KhachHangController();
        private readonly MaSanPhamController ctrlMaSanPham = new MaSanPhamController();
        private PhieuBanController ctrlPhieuBan = new PhieuBanController();
        private readonly BanLeRepository repository = new BanLeRepository();
        private readonly BindingSource dsChiTiet = new BindingSource();
        private readonly IDictionary<string, int> soLuongGocTheoLo = new Dictionary<string, int>();
        private Controll status = Controll.AddNew;
        private bool isBindingData;
        private bool daNapForm;
        private string maPhieuKhoiTao = null;

        public frmBanSi()
        {
            InitializeComponent();
        }

        public frmBanSi(string maPhieu) : this()
        {
            maPhieuKhoiTao = maPhieu;
            status = Controll.Normal;
        }

        public frmBanSi(PhieuBanController ctrlPB) : this()
        {
            ctrlPhieuBan = ctrlPB;
            status = Controll.Normal;
        }

        public void ChuyenDenPhieu(string maPhieu)
        {
            if (string.IsNullOrEmpty(maPhieu)) return;
            maPhieuKhoiTao = maPhieu;
            if (status == Controll.AddNew || status == Controll.Edit)
            {
                status = Controll.Normal;
                Allow(false);
            }
            if (bindingNavigator.BindingSource != null)
            {
                int pos = bindingNavigator.BindingSource.Find("ID", maPhieu);
                if (pos < 0)
                {
                    RicarregarPhieu();
                    pos = bindingNavigator.BindingSource.Find("ID", maPhieu);
                }
                if (pos >= 0)
                {
                    bindingNavigator.BindingSource.Position = pos;
                    DoDuLieuTuLoiLenForm();
                }
            }
        }

        private void frmNhapHang_Load(object sender, EventArgs e)
        {
            if (daNapForm)
            {
                RicarregarPhieu();
                return;
            }

            isBindingData = true;
            ctrlSanPham.HienthiAutoComboBox(cmbSanPham);
            cmbSanPham.AutoCompleteMode = AutoCompleteMode.None;
            cmbSanPham.AutoCompleteSource = AutoCompleteSource.None;
            ctrlKhachHang.HienthiAutoComboBox(cmbKhachHang, true);
            cmbSanPham.SelectedIndexChanged += cmbSanPham_SelectedIndexChanged;
            cmbMaSanPham.SelectedIndexChanged += cmbMaSanPham_SelectedIndexChanged;
            cmbCachChonLo.SelectedIndexChanged += cmbCachChonLo_SelectedIndexChanged;
            cmbCachChonLo.SelectedIndex = 0;
            cmbCachTinhGiaVon.SelectedIndex = 0;
            dsChiTiet.DataSource = TaoBangChiTiet();
            dgvDanhsachSP.DataSource = dsChiTiet;
            ctrlPhieuBan.HienthiPhieuBan(bindingNavigator, cmbKhachHang, txtMaPhieu,
                dtNgayLapPhieu, numTongTien, numDaTra, numConNo);
            dtNgayLapPhieu.MaxDate = DateTime.Now;
            GanSuKienCurrentChanged();
            isBindingData = false;
            daNapForm = true;

            if (status == Controll.AddNew)
            {
                txtMaPhieu.Text = ThamSo.LayMaPhieuBan().ToString();
                ResetChiTiet();
                Allow(true);
            }
            else
            {
                if (!string.IsNullOrEmpty(maPhieuKhoiTao) && bindingNavigator.BindingSource != null)
                {
                    int pos = bindingNavigator.BindingSource.Find("ID", maPhieuKhoiTao);
                    if (pos >= 0)
                        bindingNavigator.BindingSource.Position = pos;
                }
                Allow(false);
                DoDuLieuTuLoiLenForm();
            }
        }

        private void RicarregarPhieu()
        {
            if (status == Controll.AddNew || status == Controll.Edit)
            {
                MessageBox.Show("Hãy lưu hoặc bỏ thay đổi trước khi tải lại phiếu.", "Bán sỉ",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            isBindingData = true;
            ctrlPhieuBan = new PhieuBanController();
            ctrlPhieuBan.HienthiPhieuBan(bindingNavigator, cmbKhachHang, txtMaPhieu,
                dtNgayLapPhieu, numTongTien, numDaTra, numConNo);
            GanSuKienCurrentChanged();
            isBindingData = false;
            Allow(false);
            DoDuLieuTuLoiLenForm();
        }

        private DataTable TaoBangChiTiet()
        {
            DataTable table = new DataTable();
            table.Columns.Add("ID_MA_SAN_PHAM", typeof(string));
            table.Columns.Add("TEN_SAN_PHAM", typeof(string));
            table.Columns.Add("NGAY_HET_HAN", typeof(DateTime));
            table.Columns.Add("SO_LUONG", typeof(int));
            table.Columns.Add("DON_GIA", typeof(long));
            table.Columns.Add("THANH_TIEN", typeof(long));
            table.Columns.Add("GIA_VON", typeof(long));
            return table;
        }

        private string LayMaPhieuHienTai()
        {
            if (bindingNavigator.BindingSource != null && bindingNavigator.BindingSource.Current != null)
            {
                DataRowView row = bindingNavigator.BindingSource.Current as DataRowView;
                if (row != null && row.Row.Table.Columns.Contains("ID") && row["ID"] != DBNull.Value)
                    return Convert.ToString(row["ID"]);
            }
            return txtMaPhieu.Text;
        }

        // Refactoring: Extract Method DoDuLieuTuLoiLenForm kết hợp cờ Control Flag isBindingData
        private void DoDuLieuTuLoiLenForm()
        {
            string maPhieu = LayMaPhieuHienTai();
            if (string.IsNullOrEmpty(maPhieu)) return;
            isBindingData = true;
            try
            {
                if (txtMaPhieu.Text != maPhieu)
                    txtMaPhieu.Text = maPhieu;

                DataTable chiTiet = repository.LayChiTiet(maPhieu);
                DataTable draft = (DataTable)dsChiTiet.DataSource;
                draft.Clear();
                foreach (DataRow row in chiTiet.Rows)
                {
                    DataRow newRow = draft.NewRow();
                    newRow["ID_MA_SAN_PHAM"] = row["ID_MA_SAN_PHAM"];
                    newRow["TEN_SAN_PHAM"] = row["TEN_SAN_PHAM"];
                    newRow["NGAY_HET_HAN"] = row["NGAY_HET_HAN"];
                    newRow["SO_LUONG"] = row["SO_LUONG"];
                    newRow["DON_GIA"] = row["DON_GIA"];
                    newRow["THANH_TIEN"] = row["THANH_TIEN"];
                    newRow["GIA_VON"] = row["GIA_VON"];
                    draft.Rows.Add(newRow);
                }
            }
            finally
            {
                isBindingData = false;
            }
        }

        private void GanSuKienCurrentChanged()
        {
            if (bindingNavigator.BindingSource != null)
            {
                bindingNavigator.BindingSource.CurrentChanged -= BindingSource_CurrentChanged;
                bindingNavigator.BindingSource.CurrentChanged += BindingSource_CurrentChanged;
            }
        }

        private void BindingSource_CurrentChanged(object sender, EventArgs e)
        {
            if (status == Controll.Normal && bindingNavigator.BindingSource != null &&
                bindingNavigator.BindingSource.Current != null)
                DoDuLieuTuLoiLenForm();
        }

        private void toolXemLai_Click(object sender, EventArgs e)
        {
            RicarregarPhieu();
        }

        private void cmbSanPham_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbSanPham.SelectedValue == null || cmbSanPham.SelectedValue is DataRowView)
            {
                ResetThongTinGia();
                return;
            }
            bool baoGomHetHan = cmbCachChonLo.SelectedIndex == 1;
            ctrlMaSanPham.HienThiAutoComboBox(Convert.ToString(cmbSanPham.SelectedValue), cmbMaSanPham, baoGomHetHan);
            cmbMaSanPham.AutoCompleteMode = AutoCompleteMode.None;
            cmbMaSanPham.AutoCompleteSource = AutoCompleteSource.None;
            HienThiGiaSanPham();
        }

        private void cmbCachChonLo_SelectedIndexChanged(object sender, EventArgs e)
        {
            cmbMaSanPham.Enabled = (status == Controll.AddNew || status == Controll.Edit) && cmbCachChonLo.SelectedIndex == 1;
            if (cmbSanPham.SelectedValue != null && !(cmbSanPham.SelectedValue is DataRowView))
                cmbSanPham_SelectedIndexChanged(cmbSanPham, EventArgs.Empty);
        }

        private void cmbMaSanPham_SelectedIndexChanged(object sender, EventArgs e)
        {
            HienThiGiaSanPham();
        }

        // Refactoring: Extract Method ResetThongTinGia
        private void ResetThongTinGia()
        {
            numDonGia.Value = 0;
            numThanhTien.Value = 0;
            txtGiaNhap.Text = "0";
            txtGiaBanSi.Text = "0";
            txtGiaBanLe.Text = "0";
            txtGiaBQGQ.Text = "0";
        }

        private void HienThiGiaSanPham()
        {
            if (cmbMaSanPham.SelectedValue == null || cmbMaSanPham.SelectedValue is DataRowView)
            {
                ResetThongTinGia();
                return;
            }
            MaSanPham lo = ctrlMaSanPham.LayMaSanPham(Convert.ToString(cmbMaSanPham.SelectedValue));
            if (lo == null || lo.SanPham == null)
            {
                ResetThongTinGia();
                return;
            }
            numDonGia.Value = Math.Min(numDonGia.Maximum, lo.SanPham.GiaBanSi);
            txtGiaNhap.Text = lo.GiaNhap.ToString("#,###0");
            txtGiaBanSi.Text = lo.SanPham.GiaBanSi.ToString("#,###0");
            txtGiaBanLe.Text = lo.SanPham.GiaBanLe.ToString("#,###0");
            txtGiaBQGQ.Text = lo.SanPham.DonGiaNhap.ToString("#,###0");
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            // Guard Clauses: Kiểm tra điều kiện và thoát sớm
            if (status == Controll.Normal)
            {
                MessageBox.Show("Chọn Chỉnh sửa hoặc Thêm mới trước khi thay đổi chi tiết.");
                return;
            }
            if (cmbSanPham.SelectedValue == null || cmbSanPham.SelectedValue is DataRowView)
            {
                BaoLoi("Vui lòng chọn sản phẩm."); return;
            }
            if (cmbCachChonLo.SelectedIndex == 1 && (cmbMaSanPham.SelectedValue == null || cmbMaSanPham.SelectedValue is DataRowView))
            {
                BaoLoi("Vui lòng chọn lô xuất."); return;
            }
            if (numSoLuong.Value <= 0)
            {
                BaoLoi("Số lượng phải lớn hơn hoặc bằng 1."); return;
            }
            if (numDonGia.Value < 0)
            {
                BaoLoi("Đơn giá không được âm."); return;
            }
            try
            {
                CachChonLo cachChon = cmbCachChonLo.SelectedIndex == 0 ? CachChonLo.HetHanTruoc : CachChonLo.ChiDinh;
                CachTinhGiaVon cachTinh = cmbCachTinhGiaVon.SelectedIndex == 0 ? CachTinhGiaVon.BinhQuanGiaQuyen : CachTinhGiaVon.NhapTruocXuatTruoc;
                string maLo = cachChon == CachChonLo.ChiDinh ? Convert.ToString(cmbMaSanPham.SelectedValue) : null;
                IDictionary<string, int> soLuongDaPhanBo = LaySoLuongDraftTheoLo();
                IList<PhanBoLoXuat> phanBo = repository.LayPhanBo(Convert.ToString(cmbSanPham.SelectedValue),
                    Decimal.ToInt32(numSoLuong.Value), maLo, cachChon, cachTinh, soLuongDaPhanBo);
                long giaBan = Decimal.ToInt64(numDonGia.Value);
                long tongMoi = decimal.ToInt64(numTongTien.Value);
                foreach (PhanBoLoXuat dong in phanBo)
                {
                    dong.GiaBan = giaBan;
                    tongMoi = checked(tongMoi + dong.ThanhTien);
                }
                if (tongMoi > decimal.ToInt64(numTongTien.Maximum))
                {
                    BaoLoi("Tổng tiền vượt quá giới hạn của phiếu.");
                    return;
                }
                foreach (PhanBoLoXuat dong in phanBo) ThemDongChiTiet(dong);
                TinhLaiTongTien();
                numSoLuong.Value = 1;
                numThanhTien.Value = numDonGia.Value;
            }
            catch (Exception ex) { BaoLoi(ex.Message); }
        }

        private IDictionary<string, int> LaySoLuongDraftTheoLo()
        {
            IDictionary<string, int> soLuongTheoLo = new Dictionary<string, int>();
            DataTable table = (DataTable)dsChiTiet.DataSource;
            foreach (DataRow row in table.Rows)
            {
                string maLo = Convert.ToString(row["ID_MA_SAN_PHAM"]);
                int soLuong = Convert.ToInt32(row["SO_LUONG"]);
                int daPhanBo;
                if (soLuongTheoLo.TryGetValue(maLo, out daPhanBo))
                    soLuongTheoLo[maLo] = checked(daPhanBo + soLuong);
                else
                    soLuongTheoLo.Add(maLo, soLuong);
            }
            if (status == Controll.Edit && soLuongGocTheoLo.Count > 0)
            {
                IDictionary<string, int> soLuongChenhLech = new Dictionary<string, int>(soLuongTheoLo);
                foreach (KeyValuePair<string, int> cap in soLuongGocTheoLo)
                {
                    int daCo;
                    if (soLuongChenhLech.TryGetValue(cap.Key, out daCo))
                        soLuongChenhLech[cap.Key] = checked(daCo - cap.Value);
                    else
                        soLuongChenhLech.Add(cap.Key, -cap.Value);
                }
                return soLuongChenhLech;
            }
            return soLuongTheoLo;
        }

        private void CapNhatSoLuongGoc()
        {
            soLuongGocTheoLo.Clear();
            if (status == Controll.Edit && !String.IsNullOrEmpty(txtMaPhieu.Text))
            {
                DataTable chiTietCu = repository.LayChiTiet(txtMaPhieu.Text);
                foreach (DataRow row in chiTietCu.Rows)
                {
                    string maLo = Convert.ToString(row["ID_MA_SAN_PHAM"]);
                    int soLuong = Convert.ToInt32(row["SO_LUONG"]);
                    int daCo;
                    if (soLuongGocTheoLo.TryGetValue(maLo, out daCo))
                        soLuongGocTheoLo[maLo] = checked(daCo + soLuong);
                    else
                        soLuongGocTheoLo.Add(maLo, soLuong);
                }
            }
        }

        private void ThemDongChiTiet(PhanBoLoXuat dong)
        {
            DataTable table = (DataTable)dsChiTiet.DataSource;
            DataRow row = table.NewRow();
            row["ID_MA_SAN_PHAM"] = dong.MaLo;
            row["TEN_SAN_PHAM"] = dong.TenSanPham;
            row["NGAY_HET_HAN"] = dong.NgayHetHan;
            row["SO_LUONG"] = dong.SoLuong;
            row["DON_GIA"] = dong.GiaBan;
            row["THANH_TIEN"] = dong.ThanhTien;
            row["GIA_VON"] = dong.GiaVonTong;
            table.Rows.Add(row);
        }

        private void numDonGia_ValueChanged(object sender, EventArgs e)
        {
            decimal total = numDonGia.Value * numSoLuong.Value;
            numThanhTien.Value = Math.Min(numThanhTien.Maximum, total);
        }

        private void numTongTien_ValueChanged(object sender, EventArgs e)
        {
            if (isBindingData) return;
            if (numDaTra.Value > numTongTien.Value) numDaTra.Value = numTongTien.Value;
            numConNo.Value = numTongTien.Value - numDaTra.Value;
        }

        // Refactoring: Extract Method TinhLaiTongTien
        private void TinhLaiTongTien()
        {
            long tong = 0;
            foreach (DataRow row in ((DataTable)dsChiTiet.DataSource).Rows)
                tong = checked(tong + Convert.ToInt64(row["THANH_TIEN"]));
            if (tong > decimal.ToInt64(numTongTien.Maximum))
            {
                BaoLoi("Tổng tiền vượt quá giới hạn của phiếu.");
                return;
            }
            isBindingData = true;
            numTongTien.Value = tong;
            numConNo.Value = Math.Max(0, numTongTien.Value - numDaTra.Value);
            isBindingData = false;
        }

        private void toolLuu_Click(object sender, EventArgs e)
        {
            if (Luu())
            {
                status = Controll.Normal;
                soLuongGocTheoLo.Clear();
                Allow(false);
                ctrlPhieuBan = new PhieuBanController();
                ctrlPhieuBan.HienthiPhieuBan(bindingNavigator, cmbKhachHang, txtMaPhieu,
                    dtNgayLapPhieu, numTongTien, numDaTra, numConNo);
                GanSuKienCurrentChanged();
                DoDuLieuTuLoiLenForm();
            }
        }

        private bool Luu()
        {
            // Guard Clauses: Chống lưu Hóa đơn rỗng và kiểm tra hợp lệ
            if (cmbKhachHang.SelectedValue == null || cmbKhachHang.SelectedValue is DataRowView)
            {
                BaoLoi("Vui lòng chọn khách hàng."); return false;
            }
            DataTable table = (DataTable)dsChiTiet.DataSource;
            if (table == null || table.Rows.Count == 0)
            {
                BaoLoi("Hóa đơn phải có ít nhất một dòng sản phẩm trước khi lưu.");
                return false;
            }
            if (numTongTien.Value <= 0)
            {
                BaoLoi("Tổng tiền của hóa đơn phải lớn hơn 0 trước khi lưu.");
                return false;
            }
            try
            {
                IList<PhanBoLoXuat> chiTiet = new List<PhanBoLoXuat>();
                foreach (DataRow row in table.Rows)
                {
                    PhanBoLoXuat dong = new PhanBoLoXuat();
                    dong.MaLo = Convert.ToString(row["ID_MA_SAN_PHAM"]);
                    dong.SoLuong = Convert.ToInt32(row["SO_LUONG"]);
                    dong.GiaBan = Convert.ToInt64(row["DON_GIA"]);
                    dong.GiaVonTong = Convert.ToInt64(row["GIA_VON"]);
                    dong.GiaVonDonVi = dong.SoLuong == 0 ? 0 : dong.GiaVonTong / dong.SoLuong;
                    chiTiet.Add(dong);
                }
                bool laPhieuMoi = status == Controll.AddNew;
                long tongChiTiet = 0;
                foreach (PhanBoLoXuat dong in chiTiet)
                    tongChiTiet = checked(tongChiTiet + dong.ThanhTien);
                if (tongChiTiet > decimal.ToInt64(numTongTien.Maximum))
                {
                    BaoLoi("Tổng tiền vượt quá giới hạn của phiếu.");
                    return false;
                }
                if (numDaTra.Value > tongChiTiet)
                {
                    BaoLoi("Số tiền đã trả không được lớn hơn tổng tiền.");
                    return false;
                }
                repository.LuuPhieu(txtMaPhieu.Text, Convert.ToString(cmbKhachHang.SelectedValue),
                    dtNgayLapPhieu.Value.Date, tongChiTiet,
                    Decimal.ToInt64(numDaTra.Value), chiTiet, laPhieuMoi);
                if (laPhieuMoi && ThamSo.LaSoNguyen(txtMaPhieu.Text))
                {
                    long soPhieu = Convert.ToInt64(txtMaPhieu.Text);
                    if (soPhieu >= ThamSo.LayMaPhieuBan()) ThamSo.GanMaPhieuBan(soPhieu + 1);
                }
                return true;
            }
            catch (Exception ex) { BaoLoi(ex.Message); return false; }
        }

        private void toolLuu_Them_Click(object sender, EventArgs e)
        {
            if (status == Controll.AddNew || status == Controll.Edit)
            {
                DialogResult ketQua = MessageBox.Show("Lưu phiếu hiện tại và tạo phiếu mới? Chọn No để bỏ thay đổi.", "Bán sỉ",
                    MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                if (ketQua == DialogResult.Cancel) return;
                if (ketQua == DialogResult.Yes)
                {
                    if (!Luu()) return;
                    status = Controll.Normal;
                }
                else
                {
                    if (status == Controll.Edit)
                    {
                        ctrlPhieuBan = new PhieuBanController();
                        ctrlPhieuBan.HienthiPhieuBan(bindingNavigator, cmbKhachHang, txtMaPhieu,
                            dtNgayLapPhieu, numTongTien, numDaTra, numConNo);
                        GanSuKienCurrentChanged();
                        DoDuLieuTuLoiLenForm();
                    }
                    else
                    {
                        ResetChiTiet();
                        isBindingData = true;
                        numDaTra.Value = 0;
                        isBindingData = false;
                    }
                    status = Controll.Normal;
                }
            }
            ctrlPhieuBan = new PhieuBanController();
            status = Controll.AddNew;
            soLuongGocTheoLo.Clear();
            Allow(true);
            isBindingData = true;
            txtMaPhieu.Text = ThamSo.LayMaPhieuBan().ToString();
            dtNgayLapPhieu.Value = DateTime.Today;
            numTongTien.Value = 0;
            numDaTra.Value = 0;
            numConNo.Value = 0;
            isBindingData = false;
            ResetChiTiet();
        }

        private void ResetChiTiet()
        {
            if (status == Controll.AddNew) soLuongGocTheoLo.Clear();
            ((DataTable)dsChiTiet.DataSource).Clear();
            TinhLaiTongTien();
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            if (dsChiTiet.Current == null) return;
            if (MessageBox.Show("Xóa dòng chi tiết đang chọn?", "Bán sỉ", MessageBoxButtons.YesNo,
                MessageBoxIcon.Question) != DialogResult.Yes) return;
            dsChiTiet.RemoveCurrent();
            TinhLaiTongTien();
        }

        private void toolLuuIn_Click(object sender, EventArgs e)
        {
            if (status != Controll.Normal)
            {
                MessageBox.Show("Vui lòng lưu lại Phiếu bán hiện tại trước khi in."); return;
            }
            string maPhieu = LayMaPhieuHienTai();
            if (string.IsNullOrEmpty(maPhieu)) { BaoLoi("Không xác định được mã phiếu."); return; }
            PhieuBan phieu = ctrlPhieuBan.LayPhieuBan(maPhieu);
            if (phieu == null) { BaoLoi("Không tìm thấy phiếu bán."); return; }
            frmInPhieuBan formIn = new frmInPhieuBan(phieu);
            formIn.Show();
        }

        private void dgvDanhsachSP_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            e.Cancel = true;
        }

        private void toolChinhSua_Click(object sender, EventArgs e)
        {
            if (status != Controll.Normal) return;
            status = Controll.Edit;
            CapNhatSoLuongGoc();
            Allow(true);
        }

        private void Allow(bool value)
        {
            txtMaPhieu.Enabled = false;
            cmbKhachHang.Enabled = value && status == Controll.AddNew;
            dtNgayLapPhieu.Enabled = value;
            numDaTra.Enabled = value;
            numTongTien.Enabled = false; // Cố định tổng tiền, tính tự động theo danh sách thực tế
            cmbSanPham.Enabled = value;
            cmbMaSanPham.Enabled = value && cmbCachChonLo.SelectedIndex == 1;
            cmbCachChonLo.Enabled = value;
            cmbCachTinhGiaVon.Enabled = value;
            numDonGia.Enabled = false; // Khóa giá niêm yết, bán đúng giá hệ thống không cho sửa tay
            numSoLuong.Enabled = value;
            btnAdd.Enabled = value;
            btnRemove.Enabled = value;
            dgvDanhsachSP.Enabled = value;
        }

        private void toolThoat_Click(object sender, EventArgs e)
        {
            if (status == Controll.AddNew || status == Controll.Edit)
            {
                DialogResult result = MessageBox.Show("Lưu thay đổi trước khi thoát? Chọn No để bỏ thay đổi.",
                    "Bán sỉ", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                if (result == DialogResult.Cancel) return;
                if (result == DialogResult.Yes && !Luu()) return;
            }
            Close();
        }

        private void toolXoa_Click(object sender, EventArgs e)
        {
            if (status != Controll.Normal) return;
            if (bindingNavigator.BindingSource == null || bindingNavigator.BindingSource.Current == null) return;
            if (MessageBox.Show("Xóa phiếu bán này và hoàn tồn các lô?", "Bán sỉ",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;
            try
            {
                repository.XoaPhieu(txtMaPhieu.Text);
                bindingNavigator.BindingSource.RemoveCurrent();
                status = Controll.Normal;
            }
            catch (Exception ex) { BaoLoi(ex.Message); }
        }

        private void btnThemDaiLy_Click(object sender, EventArgs e)
        {
            frmDaiLy form = new frmDaiLy();
            form.ShowDialog();
            ctrlKhachHang.HienthiAutoComboBox(cmbKhachHang, true);
        }

        private void btnThemSanPham_Click(object sender, EventArgs e)
        {
            frmSanPham form = new frmSanPham();
            form.ShowDialog();
            ctrlSanPham.HienthiAutoComboBox(cmbSanPham);
            cmbSanPham.AutoCompleteMode = AutoCompleteMode.None;
            cmbSanPham.AutoCompleteSource = AutoCompleteSource.None;
        }

        private void BaoLoi(string message)
        {
            MessageBox.Show(message, "Bán sỉ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private void dgvDanhsachSP_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e)
        {
            e.Cancel = true;
        }
    }
}
