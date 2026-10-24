using System;
using System.Windows.Forms;
using CuahangNongduoc.Auth;
using CuahangNongduoc.Controller;

namespace CuahangNongduoc
{
    public partial class frmBaoCaoGiamGia : Form
    {
        private readonly BaoCaoGiamGiaController controller = new BaoCaoGiamGiaController();
        private System.Collections.Generic.IList<CuahangNongduoc.BusinessObject.ChiTietBaoCaoGiamGia> chiTietBaoCao;

        public frmBaoCaoGiamGia()
        {
            InitializeComponent();
            dgvNhanVien.AutoGenerateColumns = false;
            dgvHoaDon.AutoGenerateColumns = false;

            Load += (s, e) =>
            {
                if (!AuthorizationService.KiemTraVaThongBao(AuthorizationService.Permission.BaoCaoDoanhThu)) { Close(); return; }
                dtTuNgay.Value = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
                dtDenNgay.Value = DateTime.Today;
                TaiBaoCao();
            };
            dgvNhanVien.SelectionChanged += (s, e) => LocChiTietTheoNhanVien();
        }

        private void BtnXem_Click(object sender, EventArgs e) { TaiBaoCao(); }

        private void TaiBaoCao()
        {
            try
            {
                dgvNhanVien.DataSource = controller.LayTongHop(dtTuNgay.Value.Date, dtDenNgay.Value.Date);
                chiTietBaoCao = controller.LayChiTiet(dtTuNgay.Value.Date, dtDenNgay.Value.Date);
                dgvHoaDon.DataSource = chiTietBaoCao;
                DinhDangCot(dgvNhanVien); DinhDangCot(dgvHoaDon);
            }
            catch (Exception ex) { MessageBox.Show("Không tải được báo cáo. Hãy kiểm tra migration và kết nối cơ sở dữ liệu.\n" + ex.Message, "Báo cáo giảm giá", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }

        private void LocChiTietTheoNhanVien()
        {
            if (dgvNhanVien.CurrentRow == null || dgvHoaDon.DataSource == null) return;
            var id = dgvNhanVien.CurrentRow.Cells["colNvId"].Value;
            if (chiTietBaoCao == null) return;
            var view = id == null || id == DBNull.Value
                ? new System.Collections.Generic.List<CuahangNongduoc.BusinessObject.ChiTietBaoCaoGiamGia>(chiTietBaoCao).FindAll(x => !x.IdNguoiDung.HasValue)
                : new System.Collections.Generic.List<CuahangNongduoc.BusinessObject.ChiTietBaoCaoGiamGia>(chiTietBaoCao).FindAll(x => x.IdNguoiDung == Convert.ToInt32(id));
            dgvHoaDon.DataSource = view;
        }

        private static void DinhDangCot(DataGridView grid)
        {
            foreach (DataGridViewColumn col in grid.Columns)
            {
                string property = col.DataPropertyName;
                if (property == "IdNguoiDung") col.Visible = false;
                if (property == "TenDangNhap") { col.Visible = false; col.HeaderText = "Tài khoản"; }
                if (property == "HoTen") col.HeaderText = "Nhân viên lập hóa đơn";
                if (property == "SoHoaDon") col.HeaderText = "Số hóa đơn";
                if (property == "TongTienHang") col.HeaderText = "Tổng tiền hàng";
                if (property == "TongTienGiam") col.HeaderText = "Tổng tiền giảm";
                if (property == "TongThanhToan") col.HeaderText = "Tổng thanh toán";
                if (property == "IdPhieuBan") col.HeaderText = "Mã hóa đơn";
                if (property == "NgayBan") { col.HeaderText = "Ngày bán"; col.DefaultCellStyle.Format = "dd/MM/yyyy"; }
                if (property == "LoaiGiamGia") col.HeaderText = "Loại giảm";
                if (property == "TyLeGiamGia") col.HeaderText = "Tỷ lệ giảm (%)";
                if (property == "TienHang") col.HeaderText = "Tiền hàng";
                if (property == "TienGiamGia") col.HeaderText = "Tiền giảm";
                if (property == "TongTien") col.HeaderText = "Tổng thanh toán";
                if (property == "TongTienHang" || property == "TongTienGiam" || property == "TongThanhToan" || property == "TienHang" || property == "TienGiamGia" || property == "TongTien") col.DefaultCellStyle.Format = "N0";
            }
        }
    }

}
