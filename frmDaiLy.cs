using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using CuahangNongduoc.Controller;

namespace CuahangNongduoc
{
    public partial class frmDaiLy : Form
    {
        private const string PLACEHOLDER_TIM_TEN = "Tìm theo Tên đại lý";
        private const string PLACEHOLDER_TIM_DIACHI = "Tìm theo Địa chỉ";
        private const int MAX_LEN_HOTEN = 100;
        private const int MAX_LEN_DIACHI = 250;
        private const int MAX_LEN_DIENTHOAI = 15;

        private readonly KhachHangController ctrl = new KhachHangController();

        public frmDaiLy()
        {
            InitializeComponent();
        }

        private void frmDaiLy_Load(object sender, EventArgs e)
        {
            ctrl.HienthiDaiLyDataGridview(dataGridView, bindingNavigator);
            CauHinhCotDataGridView();
        }

        private void CauHinhCotDataGridView()
        {
            if (dataGridView.Columns.Contains("colID"))
                dataGridView.Columns["colID"].ReadOnly = true;

            if (dataGridView.Columns.Contains("colHoTen") && dataGridView.Columns["colHoTen"] is DataGridViewTextBoxColumn colTen)
                colTen.MaxInputLength = MAX_LEN_HOTEN;

            if (dataGridView.Columns.Contains("colDiaChi") && dataGridView.Columns["colDiaChi"] is DataGridViewTextBoxColumn colDC)
                colDC.MaxInputLength = MAX_LEN_DIACHI;

            if (dataGridView.Columns.Contains("colDienThoai") && dataGridView.Columns["colDienThoai"] is DataGridViewTextBoxColumn colDT)
                colDT.MaxInputLength = MAX_LEN_DIENTHOAI;

            dataGridView.DataError -= dataGridView_DataError;
            dataGridView.DataError += dataGridView_DataError;
        }

        private void dataGridView_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            e.Cancel = true;
            MessageBox.Show("Dữ liệu nhập vào ô không đúng định dạng!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private void bindingNavigatorAddNewItem_Click(object sender, EventArgs e)
        {
            long maso = ThamSo.KhachHang;
            ThamSo.KhachHang = maso + 1;

            DataRowView row = (DataRowView)bindingNavigator.BindingSource.AddNew();
            row["ID"] = maso;
            row["LOAI_KH"] = true;
            row["HO_TEN"] = "";
            row["DIA_CHI"] = "";
            row["DIEN_THOAI"] = "";
        }

        private bool KiemTraHopLe()
        {
            dataGridView.EndEdit();
            if (bindingNavigator.BindingSource != null)
            {
                bindingNavigator.BindingSource.EndEdit();
            }

            foreach (DataGridViewRow row in dataGridView.Rows)
            {
                if (row.IsNewRow) continue;

                string tenDaiLy = Convert.ToString(row.Cells["colHoTen"].Value).Trim();
                if (string.IsNullOrEmpty(tenDaiLy))
                {
                    MessageBox.Show("Tên đại lý không được để trống!", "Lỗi nhập liệu", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    dataGridView.CurrentCell = row.Cells["colHoTen"];
                    dataGridView.BeginEdit(true);
                    return false;
                }

                string dienThoai = Convert.ToString(row.Cells["colDienThoai"].Value).Trim();
                if (!string.IsNullOrEmpty(dienThoai))
                {
                    foreach (char c in dienThoai)
                    {
                        if (!char.IsDigit(c) && c != ' ' && c != '-' && c != '+')
                        {
                            MessageBox.Show("Số điện thoại của đại lý '" + tenDaiLy + "' chỉ được chứa chữ số!", "Lỗi nhập liệu", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            dataGridView.CurrentCell = row.Cells["colDienThoai"];
                            dataGridView.BeginEdit(true);
                            return false;
                        }
                    }
                }
            }
            return true;
        }

        private void toolLuu_Click(object sender, EventArgs e)
        {
            bindingNavigatorPositionItem.Focus();

            if (!KiemTraHopLe()) return;

            LuuDuLieu(true);
        }

        private void LuuDuLieu(bool hienThongBao)
        {
            try
            {
                ctrl.Save();
                if (hienThongBao)
                {
                    MessageBox.Show("Đã lưu dữ liệu đại lý thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Có lỗi xảy ra khi lưu dữ liệu: " + ex.Message, "Lỗi hệ thống", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void toolThoat_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private bool XacNhanXoa()
        {
            return MessageBox.Show("Bạn có chắc chắn muốn xóa đại lý này không?", "Đại Lý", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes;
        }

        private void bindingNavigatorDeleteItem_Click(object sender, EventArgs e)
        {
            if (bindingNavigator.BindingSource == null || bindingNavigator.BindingSource.Current == null)
            {
                MessageBox.Show("Không có đại lý nào được chọn để xóa!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (XacNhanXoa())
            {
                bindingNavigator.BindingSource.RemoveCurrent();
                LuuDuLieu(false);
            }
        }

        private void dataGridView_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e)
        {
            if (!XacNhanXoa())
            {
                e.Cancel = true;
            }
        }

        private void toolTimHoTen_Click(object sender, EventArgs e)
        {
            toolTimHoTen.Checked = true;
            toolTimDiaChi.Checked = false;
            toolTimKhachHang.Text = PLACEHOLDER_TIM_TEN;
            toolTimKhachHang.ForeColor = Color.FromArgb(224, 224, 224);
            bindingNavigator.Focus();
        }

        private void toolTimDiaChi_Click(object sender, EventArgs e)
        {
            toolTimHoTen.Checked = false;
            toolTimDiaChi.Checked = true;
            toolTimKhachHang.Text = PLACEHOLDER_TIM_DIACHI;
            toolTimKhachHang.ForeColor = Color.FromArgb(224, 224, 224);
            bindingNavigator.Focus();
        }

        private void toolTimKhachHang_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                e.Handled = true;
                string tuKhoa = toolTimKhachHang.Text.Trim();

                if (tuKhoa == PLACEHOLDER_TIM_TEN || tuKhoa == PLACEHOLDER_TIM_DIACHI)
                {
                    tuKhoa = "";
                }

                if (toolTimHoTen.Checked)
                    ctrl.TimHoTen(tuKhoa, true);
                else
                    ctrl.TimDiaChi(tuKhoa, true);
            }
        }

        private void toolTimKhachHang_Leave(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(toolTimKhachHang.Text.Trim()))
            {
                toolTimKhachHang.Text = toolTimHoTen.Checked ? PLACEHOLDER_TIM_TEN : PLACEHOLDER_TIM_DIACHI;
                toolTimKhachHang.ForeColor = Color.FromArgb(224, 224, 224);
            }
        }

        private void toolTimKhachHang_Enter(object sender, EventArgs e)
        {
            if (toolTimKhachHang.Text == PLACEHOLDER_TIM_TEN || toolTimKhachHang.Text == PLACEHOLDER_TIM_DIACHI)
            {
                toolTimKhachHang.Text = "";
            }
            toolTimKhachHang.ForeColor = Color.Black;
        }
    }
}