using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace CuahangNongduoc
{
    public partial class frmNhaCungCap : Form
    {
        CuahangNongduoc.Controller.NhaCungCapController ctrl = new CuahangNongduoc.Controller.NhaCungCapController();
        public frmNhaCungCap()
        {
            InitializeComponent();
            bindingNavigator.AddNewItem = null;
            dataGridView.EditingControlShowing += dataGridView_EditingControlShowing;
            dataGridView.CellValidating += dataGridView_CellValidating;
        }

        private void dataGridView_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e)
        {
            if (MessageBox.Show("Bạn có chắc chắn xóa không?", "Nha Cung Cap", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
            {
                e.Cancel = true;
            }
        }

        private void bindingNavigatorDeleteItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Bạn có chắc chắn xóa không?", "Nha Cung Cap", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                bindingNavigator.BindingSource.RemoveCurrent();
            }
        }

        private void frmNhaCungCap_Load(object sender, EventArgs e)
        {
            ctrl.HienthiDataGridview(dataGridView, bindingNavigator);
            long maxId = 0;
            DataTable tbl = bindingNavigator.BindingSource.DataSource as DataTable;

            if (tbl != null)
            {
                foreach (DataRow row in tbl.Rows)
                {
                    if (row.RowState == DataRowState.Deleted)
                        continue;

                    long id;
                    if (long.TryParse(Convert.ToString(row["ID"]), out id))
                    {
                        if (id > maxId)
                            maxId = id;
                    }
                }
            }

            ThamSo.NhaCungCap = maxId + 1;
        }


        private void bindingNavigatorAddNewItem_Click(object sender, EventArgs e)
        {
            BindingSource bs = bindingNavigator.BindingSource;
            DataTable tbl = bs.DataSource as DataTable;

            if (tbl == null)
                return;

            long maxId = 0;

            foreach (DataRow r in tbl.Rows)
            {
                if (r.RowState == DataRowState.Deleted)
                    continue;

                long id;

                if (long.TryParse(Convert.ToString(r["ID"]), out id))
                {
                    if (id > maxId)
                        maxId = id;
                }
            }

            long newId = maxId + 1;

            bool trungMa;

            do
            {
                trungMa = false;

                foreach (DataRow r in tbl.Rows)
                {
                    if (r.RowState == DataRowState.Deleted)
                        continue;

                    if (Convert.ToString(r["ID"]) == newId.ToString())
                    {
                        trungMa = true;
                        newId++;
                        break;
                    }
                }
            }
            while (trungMa);

            DataRowView newRow = (DataRowView)bs.AddNew();
            newRow["ID"] = newId;

            ThamSo.NhaCungCap = newId + 1;
        }


        private void toolLuu_Click(object sender, EventArgs e)
        {
            bindingNavigatorPositionItem.Focus();
            ctrl.Save();
        }

        private void toolThoat_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void toolTimNhaCungCap_Enter(object sender, EventArgs e)
        {
            toolTimNhaCungCap.Text = "";
            toolTimNhaCungCap.ForeColor = Color.Black;
        }

        private void toolTimNhaCungCap_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13)
            {
                if (toolTimHoTen.Checked)
                {
                    ctrl.TimHoTen(toolTimNhaCungCap.Text);
                }
                else
                {
                    ctrl.TimDiaChi(toolTimNhaCungCap.Text);
                }
            }
        }

        private void toolTimNhaCungCap_Leave(object sender, EventArgs e)
        {
            if (toolTimHoTen.Checked == true)
                toolTimNhaCungCap.Text = "Tìm theo Nhà cung cấp";
            else
                toolTimNhaCungCap.Text = "Tìm theo Địa chỉ";

            toolTimNhaCungCap.ForeColor = Color.FromArgb(224, 224, 224);
        }

        private void toolTimHoTen_Click(object sender, EventArgs e)
        {
            toolTimDiaChi.Checked = !toolTimDiaChi.Checked;
            toolTimHoTen.Checked = !toolTimDiaChi.Checked;
            toolTimNhaCungCap.Text = "Tìm theo Nhà cung cấp";
            bindingNavigator.Focus();
        }

        private void toolTimDiaChi_Click(object sender, EventArgs e)
        {
            toolTimHoTen.Checked = !toolTimHoTen.Checked;
            toolTimDiaChi.Checked = !toolTimHoTen.Checked;
            toolTimNhaCungCap.Text = "Tìm theo Địa chỉ";
            bindingNavigator.Focus();
        }
        private void dataGridView_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            if (e.Control is TextBox tb)
            {
                tb.KeyPress -= DienThoai_KeyPress;

                if (dataGridView.CurrentCell != null)
                {
                    string columnName =
                        dataGridView.Columns[
                            dataGridView.CurrentCell.ColumnIndex
                        ].DataPropertyName;

                    if (columnName == "DIEN_THOAI")
                    {
                        tb.KeyPress += DienThoai_KeyPress;
                    }
                }
            }
        }

        private void DienThoai_KeyPress(
            object sender,
            KeyPressEventArgs e)
        {
            // Cho phép phím điều khiển và các chữ số 0-9
            if (!char.IsControl(e.KeyChar) &&
                (e.KeyChar < '0' || e.KeyChar > '9'))
            {
                e.Handled = true;
            }
        }
        private void dataGridView_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            string columnName =
                dataGridView.Columns[e.ColumnIndex].DataPropertyName;

            if (columnName != "DIEN_THOAI")
                return;

            string phone = Convert.ToString(e.FormattedValue);

            // Cho phép để trống; nếu có nhập thì chỉ được chứa 0-9
            if (!string.IsNullOrEmpty(phone) &&
                !Regex.IsMatch(phone, @"^[0-9]+$"))
            {
                MessageBox.Show(
                    "Số điện thoại chỉ được nhập các chữ số từ 0 đến 9!",
                    "Dữ liệu không hợp lệ",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                e.Cancel = true;
            }
        }

        private void btnDS_Click(object sender, EventArgs e)
        {
            ctrl.HienthiDataGridview(dataGridView, bindingNavigator);
        }
    }
}