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
    /// <summary>
    /// Form quan ly don vi tinh - cho phep xem, them, sua, xoa don vi tinh.
    /// Refactored: them day du xu ly su kien Them/Xoa, validation, xac nhan xoa.
    /// </summary>
    public partial class frmDonViTinh : Form
    {
        // =====================================================================
        // FIELDS
        // =====================================================================
        private readonly DonViTinhController ctrl = new DonViTinhController();

        // =====================================================================
        // CONSTRUCTOR
        // =====================================================================
        public frmDonViTinh()
        {
            InitializeComponent();
        }

        // =====================================================================
        // LOAD FORM
        // =====================================================================
        private void frmDonViTinh_Load(object sender, EventArgs e)
        {
            NapDanhSachDonViTinh();
            CauHinhBindingNavigator();
        }

        // =====================================================================
        // PRIVATE METHODS - Extract Method
        // =====================================================================

        /// <summary>
        /// Nap danh sach don vi tinh len DataGridView.
        /// </summary>
        private void NapDanhSachDonViTinh()
        {
            ctrl.HienthiDataGridview(dataGridView, bindingNavigator);
        }

        /// <summary>
        /// Cau hinh BindingNavigator: gan su kien Them va Xoa tuy chinh
        /// thay vi de mac dinh (mac dinh khong co Save va khong co xac nhan).
        /// </summary>
        private void CauHinhBindingNavigator()
        {
            // Gan su kien cho nut Them va Xoa tren BindingNavigator
            bindingNavigatorAddNewItem.Click += new EventHandler(bindingNavigatorAddNewItem_Click);
            bindingNavigatorDeleteItem.Click += new EventHandler(bindingNavigatorDeleteItem_Click);
        }

        /// <summary>
        /// Kiem tra ten don vi tinh co hop le (khong trong, khong qua dai).
        /// Guard Clause pattern.
        /// </summary>
        private bool DuLieuHienTaiHopLe()
        {
            if (bindingNavigator.BindingSource == null ||
                bindingNavigator.BindingSource.Current == null)
                return false;

            DataRowView rowView = bindingNavigator.BindingSource.Current as DataRowView;
            if (rowView == null) return false;

            string tenDonVi = rowView["TEN_DON_VI"].ToString().Trim();

            if (string.IsNullOrWhiteSpace(tenDonVi))
            {
                MessageBox.Show("Ten don vi tinh khong duoc de trong.", "Thong bao",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (tenDonVi.Length > 50)
            {
                MessageBox.Show("Ten don vi tinh khong duoc qua 50 ky tu.", "Thong bao",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            return true;
        }

        // =====================================================================
        // SU KIEN TOOLBAR - LUU
        // =====================================================================
        private void toolLuu_Click(object sender, EventArgs e)
        {
            // FIX: Commit pending edits truoc khi luu
            dataGridView.EndEdit();
            bindingNavigator.BindingSource.EndEdit();

            if (!DuLieuHienTaiHopLe()) return;

            bool luuThanhCong = ctrl.Save();
            if (luuThanhCong)
            {
                MessageBox.Show("Luu don vi tinh thanh cong.", "Thong bao",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("Khong co thay doi nao duoc luu.", "Thong bao",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }


        // =====================================================================
        // SU KIEN BINDINGNAVIGATOR - THEM MOI
        // =====================================================================
        /// <summary>
        /// Xu ly them dong moi vao BindingSource (giao dien).
        /// Nguoi dung nhap ten roi nhan Luu de luu xuong DB.
        /// </summary>
        private void bindingNavigatorAddNewItem_Click(object sender, EventArgs e)
        {
            // BindingSource tu dong them dong moi khi AddNew duoc goi
            // Viec nay xay ra tu dong qua BindingNavigator
            // Su kien nay chi can dam bao DataGridView o trang thai chinh sua
            if (bindingNavigator.BindingSource != null)
            {
                bindingNavigator.BindingSource.AddNew();
                bindingNavigator.BindingSource.MoveLast();
            }
        }

        // =====================================================================
        // SU KIEN BINDINGNAVIGATOR - XOA
        // =====================================================================
        /// <summary>
        /// Xoa don vi tinh hien tai sau khi xac nhan.
        /// LUI CU: BindingNavigator mac dinh xoa khong hoi xac nhan va khong Save.
        /// </summary>
        private void bindingNavigatorDeleteItem_Click(object sender, EventArgs e)
        {
            // Guard clause: khong co du lieu thi bo qua
            if (bindingNavigator.BindingSource == null ||
                bindingNavigator.BindingSource.Count == 0)
                return;

            var xacNhan = MessageBox.Show(
                "Ban co chac chan muon xoa don vi tinh nay khong?\n" +
                "Luu y: Khong the xoa neu don vi tinh dang duoc su dung boi san pham.",
                "Xac nhan xoa",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (xacNhan != DialogResult.Yes) return;

            bindingNavigator.BindingSource.RemoveCurrent();

            bool xoaThanhCong = ctrl.Save();
            if (!xoaThanhCong)
            {
                MessageBox.Show(
                    "Xoa that bai. Don vi tinh nay co the dang duoc su dung boi san pham.",
                    "Loi xoa",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                // FIX: Hoan nguyen row Deleted ve Unchanged de du lieu hien thi day du
                ctrl.RejectChanges();
            }
        }
    }
}
