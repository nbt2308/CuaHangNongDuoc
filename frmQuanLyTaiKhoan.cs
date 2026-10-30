using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using CuahangNongduoc.Auth;

namespace CuahangNongduoc
{
    /// <summary>
    /// Form quản lý tài khoản người dùng - chỉ dành cho Admin.
    /// Cho phép xem, thêm, sửa, khóa, mở khóa và đặt lại mật khẩu tài khoản.
    /// </summary>
    public partial class frmQuanLyTaiKhoan : Form
    {
        private readonly NguoiDungData _data;
        private List<NguoiDung> _danhSach;
        private NguoiDung _taiKhoanDangChon = null;

        public frmQuanLyTaiKhoan()
        {
            InitializeComponent();
            _data = new NguoiDungData();
        }

        private void frmQuanLyTaiKhoan_Load(object sender, EventArgs e)
        {
            // Kiểm tra quyền Admin
            if (!AuthorizationService.CoQuyen(AuthorizationService.Permission.QuanLyTaiKhoan))
            {
                MessageBox.Show("Chỉ Quản trị viên mới có quyền truy cập chức năng này.",
                    "Từ chối truy cập", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                this.Close();
                return;
            }

            // Cấu hình DataGridView
            CauHinhGrid();
            TaiDanhSach();
        }

        private void CauHinhGrid()
        {
            dgvTaiKhoan.AutoGenerateColumns = false;
            dgvTaiKhoan.Columns.Clear();
            dgvTaiKhoan.Columns.Add(new DataGridViewTextBoxColumn { Name = "colId", HeaderText = "ID", DataPropertyName = "Id", Width = 45, ReadOnly = true, SortMode = DataGridViewColumnSortMode.Automatic });
            dgvTaiKhoan.Columns.Add(new DataGridViewTextBoxColumn { Name = "colTen", HeaderText = "Tên đăng nhập", DataPropertyName = "TenDangNhap", Width = 150, ReadOnly = true, SortMode = DataGridViewColumnSortMode.Automatic  });
            dgvTaiKhoan.Columns.Add(new DataGridViewTextBoxColumn { Name = "colHoTen", HeaderText = "Họ tên", DataPropertyName = "HoTen", Width = 200, ReadOnly = true, SortMode = DataGridViewColumnSortMode.Automatic });
            dgvTaiKhoan.Columns.Add(new DataGridViewTextBoxColumn { Name = "colRole", HeaderText = "Vai trò", DataPropertyName = "RoleDisplayName", Width = 160, ReadOnly = true, SortMode = DataGridViewColumnSortMode.Automatic });
            dgvTaiKhoan.Columns.Add(new DataGridViewCheckBoxColumn { Name = "colActive", HeaderText = "Hoạt động", DataPropertyName = "IsActive", Width = 90, ReadOnly = true, SortMode = DataGridViewColumnSortMode.Automatic });
            dgvTaiKhoan.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvTaiKhoan.MultiSelect = false;
            dgvTaiKhoan.ReadOnly = true;
            dgvTaiKhoan.AllowUserToAddRows = false;
            dgvTaiKhoan.RowHeadersVisible = false;
            dgvTaiKhoan.BackgroundColor = Color.White;
            dgvTaiKhoan.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(245, 255, 245);
            dgvTaiKhoan.SelectionChanged += new EventHandler(dgvTaiKhoan_SelectionChanged);
        }

        private void TaiDanhSach(string tuKhoa = null)
        {
            try
            {
                _danhSach = _data.LayTatCa();
                if (!string.IsNullOrEmpty(tuKhoa))
                {
                    string kw = tuKhoa.ToLower();
                    _danhSach = _danhSach.FindAll(x =>
                        (x.TenDangNhap ?? "").ToLower().Contains(kw) ||
                        (x.HoTen ?? "").ToLower().Contains(kw));
                }
                dgvTaiKhoan.DataSource = null;
                dgvTaiKhoan.DataSource = _danhSach;
                lblSoLuong.Text = string.Format("Tổng: {0} tài khoản", _danhSach.Count);
                CapNhatTrangThaiNut();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải danh sách: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dgvTaiKhoan_SelectionChanged(object sender, EventArgs e)
        {
            _taiKhoanDangChon = null;
            if (dgvTaiKhoan.SelectedRows.Count > 0)
                _taiKhoanDangChon = dgvTaiKhoan.SelectedRows[0].DataBoundItem as NguoiDung;
            CapNhatTrangThaiNut();
        }

        private void CapNhatTrangThaiNut()
        {
            bool coChon = _taiKhoanDangChon != null;
            btnSua.Enabled = coChon;
            btnKhoaMoKhoa.Enabled = coChon;
            btnDatLaiMatKhau.Enabled = coChon;

            if (coChon && _taiKhoanDangChon != null)
            {
                btnKhoaMoKhoa.Text = _taiKhoanDangChon.IsActive ? "🔒 Khóa TK" : "🔓 Mở khóa TK";
                btnKhoaMoKhoa.BackColor = _taiKhoanDangChon.IsActive
                    ? Color.FromArgb(200, 80, 80)
                    : Color.FromArgb(30, 130, 70);
            }
        }

        private void btnThemMoi_Click(object sender, EventArgs e)
        {
            using (var frm = new frmTaiKhoanChiTiet(null))
            {
                if (frm.ShowDialog(this) == DialogResult.OK)
                    TaiDanhSach(txtTimKiem.Text);
            }
        }

        private void btnSua_Click(object sender, EventArgs e)
        {
            if (_taiKhoanDangChon == null) return;
            // Load đầy đủ thông tin (không có hash)
            using (var frm = new frmTaiKhoanChiTiet(_taiKhoanDangChon))
            {
                if (frm.ShowDialog(this) == DialogResult.OK)
                    TaiDanhSach(txtTimKiem.Text);
            }
        }

        private void btnKhoaMoKhoa_Click(object sender, EventArgs e)
        {
            if (_taiKhoanDangChon == null) return;

            bool moiTrangThai = !_taiKhoanDangChon.IsActive;

            // Ngăn không cho khóa tài khoản Admin đang đăng nhập
            if (!moiTrangThai && _taiKhoanDangChon.Id == UserSession.UserId)
            {
                MessageBox.Show("Không thể khóa tài khoản đang đăng nhập của chính bạn.",
                    "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Kiểm tra không để hệ thống mất Admin cuối cùng
            if (!moiTrangThai && string.Equals(_taiKhoanDangChon.Role, "admin", StringComparison.OrdinalIgnoreCase))
            {
                int soAdmin = _data.DemAdminDangHoatDong();
                if (soAdmin <= 1)
                {
                    MessageBox.Show("Không thể khóa tài khoản Admin cuối cùng đang hoạt động.\nHệ thống cần ít nhất một Admin để quản trị.",
                        "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }

            string hanhDong = moiTrangThai ? "mở khóa" : "khóa";
            if (MessageBox.Show(
                string.Format("Bạn có chắc muốn {0} tài khoản '{1}'?", hanhDong, _taiKhoanDangChon.TenDangNhap),
                "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                try
                {
                    _data.DatTrangThai(_taiKhoanDangChon.Id, moiTrangThai);
                    TaiDanhSach(txtTimKiem.Text);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnDatLaiMatKhau_Click(object sender, EventArgs e)
        {
            if (_taiKhoanDangChon == null) return;

            using (var frm = new frmDatLaiMatKhau(_taiKhoanDangChon.Id, _taiKhoanDangChon.TenDangNhap))
            {
                frm.ShowDialog(this);
            }
        }

        private void btnTimKiem_Click(object sender, EventArgs e)
        {
            TaiDanhSach(txtTimKiem.Text);
        }

        private void txtTimKiem_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                TaiDanhSach(txtTimKiem.Text);
        }

        private void btnLamMoi_Click(object sender, EventArgs e)
        {
            txtTimKiem.Clear();
            TaiDanhSach();
        }

        private void btnDong_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (_taiKhoanDangChon == null) return;

            if ( _taiKhoanDangChon.Id == UserSession.UserId)
            {
                MessageBox.Show("Không thể xóa tài khoản đang đăng nhập của chính bạn.",
                    "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Kiểm tra không để hệ thống mất Admin cuối cùng
            if (MessageBox.Show(
                string.Format("Bạn có chắc muốn xóa tài khoản '{0}'?", _taiKhoanDangChon.TenDangNhap),
                "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                try
                {
                    _data.XoaNguoiDung(_taiKhoanDangChon.Id);
                    TaiDanhSach(txtTimKiem.Text);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}
