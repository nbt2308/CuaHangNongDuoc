namespace CuahangNongduoc
{
    partial class frmQuanLyTaiKhoan
    {
        private System.ComponentModel.IContainer components = null;
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code
        private void InitializeComponent()
        {
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            this.panelTop = new System.Windows.Forms.Panel();
            this.lblTitle = new System.Windows.Forms.Label();
            this.panelSearch = new System.Windows.Forms.Panel();
            this.lblTimKiem = new System.Windows.Forms.Label();
            this.txtTimKiem = new System.Windows.Forms.TextBox();
            this.btnTimKiem = new System.Windows.Forms.Button();
            this.btnLamMoi = new System.Windows.Forms.Button();
            this.lblSoLuong = new System.Windows.Forms.Label();
            this.dgvTaiKhoan = new System.Windows.Forms.DataGridView();
            this.panelButtons = new System.Windows.Forms.Panel();
            this.btnXoaNguoiDung = new System.Windows.Forms.Button();
            this.btnThemMoi = new System.Windows.Forms.Button();
            this.btnSua = new System.Windows.Forms.Button();
            this.btnKhoaMoKhoa = new System.Windows.Forms.Button();
            this.btnDatLaiMatKhau = new System.Windows.Forms.Button();
            this.btnDong = new System.Windows.Forms.Button();
            this.panelTop.SuspendLayout();
            this.panelSearch.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvTaiKhoan)).BeginInit();
            this.panelButtons.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelTop
            // 
            this.panelTop.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(130)))), ((int)(((byte)(70)))));
            this.panelTop.Controls.Add(this.lblTitle);
            this.panelTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelTop.Location = new System.Drawing.Point(0, 0);
            this.panelTop.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.panelTop.Name = "panelTop";
            this.panelTop.Size = new System.Drawing.Size(1063, 68);
            this.panelTop.TabIndex = 5;
            // 
            // lblTitle
            // 
            this.lblTitle.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.lblTitle.ForeColor = System.Drawing.Color.White;
            this.lblTitle.Location = new System.Drawing.Point(0, 0);
            this.lblTitle.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Padding = new System.Windows.Forms.Padding(20, 0, 0, 0);
            this.lblTitle.Size = new System.Drawing.Size(1063, 68);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "👥  QUẢN LÝ TÀI KHOẢN NGƯỜI DÙNG";
            this.lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // panelSearch
            // 
            this.panelSearch.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(250)))), ((int)(((byte)(245)))));
            this.panelSearch.Controls.Add(this.lblTimKiem);
            this.panelSearch.Controls.Add(this.txtTimKiem);
            this.panelSearch.Controls.Add(this.btnTimKiem);
            this.panelSearch.Controls.Add(this.btnLamMoi);
            this.panelSearch.Controls.Add(this.lblSoLuong);
            this.panelSearch.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelSearch.Location = new System.Drawing.Point(0, 68);
            this.panelSearch.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.panelSearch.Name = "panelSearch";
            this.panelSearch.Padding = new System.Windows.Forms.Padding(13, 12, 13, 6);
            this.panelSearch.Size = new System.Drawing.Size(1063, 68);
            this.panelSearch.TabIndex = 4;
            // 
            // lblTimKiem
            // 
            this.lblTimKiem.AutoSize = true;
            this.lblTimKiem.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblTimKiem.Location = new System.Drawing.Point(13, 22);
            this.lblTimKiem.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblTimKiem.Name = "lblTimKiem";
            this.lblTimKiem.Size = new System.Drawing.Size(73, 20);
            this.lblTimKiem.TabIndex = 0;
            this.lblTimKiem.Text = "Tìm kiếm:";
            // 
            // txtTimKiem
            // 
            this.txtTimKiem.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtTimKiem.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.txtTimKiem.Location = new System.Drawing.Point(100, 18);
            this.txtTimKiem.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.txtTimKiem.Name = "txtTimKiem";
            this.txtTimKiem.Size = new System.Drawing.Size(558, 27);
            this.txtTimKiem.TabIndex = 1;
            this.txtTimKiem.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtTimKiem_KeyDown);
            // 
            // btnTimKiem
            // 
            this.btnTimKiem.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(130)))), ((int)(((byte)(70)))));
            this.btnTimKiem.FlatAppearance.BorderSize = 0;
            this.btnTimKiem.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnTimKiem.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.btnTimKiem.ForeColor = System.Drawing.Color.White;
            this.btnTimKiem.Location = new System.Drawing.Point(667, 18);
            this.btnTimKiem.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnTimKiem.Name = "btnTimKiem";
            this.btnTimKiem.Size = new System.Drawing.Size(107, 30);
            this.btnTimKiem.TabIndex = 2;
            this.btnTimKiem.Text = "Tìm";
            this.btnTimKiem.UseVisualStyleBackColor = false;
            this.btnTimKiem.Click += new System.EventHandler(this.btnTimKiem_Click);
            // 
            // btnLamMoi
            // 
            this.btnLamMoi.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(180)))), ((int)(((byte)(180)))), ((int)(((byte)(180)))));
            this.btnLamMoi.FlatAppearance.BorderSize = 0;
            this.btnLamMoi.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnLamMoi.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.btnLamMoi.Location = new System.Drawing.Point(787, 18);
            this.btnLamMoi.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnLamMoi.Name = "btnLamMoi";
            this.btnLamMoi.Size = new System.Drawing.Size(107, 30);
            this.btnLamMoi.TabIndex = 3;
            this.btnLamMoi.Text = "Làm mới";
            this.btnLamMoi.UseVisualStyleBackColor = false;
            this.btnLamMoi.Click += new System.EventHandler(this.btnLamMoi_Click);
            // 
            // lblSoLuong
            // 
            this.lblSoLuong.AutoSize = true;
            this.lblSoLuong.Font = new System.Drawing.Font("Segoe UI", 8.5F);
            this.lblSoLuong.ForeColor = System.Drawing.Color.Gray;
            this.lblSoLuong.Location = new System.Drawing.Point(913, 23);
            this.lblSoLuong.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblSoLuong.Name = "lblSoLuong";
            this.lblSoLuong.Size = new System.Drawing.Size(123, 20);
            this.lblSoLuong.TabIndex = 4;
            this.lblSoLuong.Text = "Tổng: 0 tài khoản";
            // 
            // dgvTaiKhoan
            // 
            this.dgvTaiKhoan.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvTaiKhoan.BorderStyle = System.Windows.Forms.BorderStyle.None;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(130)))), ((int)(((byte)(70)))));
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Segoe UI", 9F);
            dataGridViewCellStyle1.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvTaiKhoan.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvTaiKhoan.ColumnHeadersHeight = 29;
            this.dgvTaiKhoan.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvTaiKhoan.EnableHeadersVisualStyles = false;
            this.dgvTaiKhoan.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.dgvTaiKhoan.Location = new System.Drawing.Point(0, 136);
            this.dgvTaiKhoan.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.dgvTaiKhoan.MultiSelect = false;
            this.dgvTaiKhoan.Name = "dgvTaiKhoan";
            this.dgvTaiKhoan.RowHeadersWidth = 51;
            this.dgvTaiKhoan.RowTemplate.Height = 28;
            this.dgvTaiKhoan.Size = new System.Drawing.Size(1063, 411);
            this.dgvTaiKhoan.TabIndex = 3;
            // 
            // panelButtons
            // 
            this.panelButtons.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(250)))), ((int)(((byte)(245)))));
            this.panelButtons.Controls.Add(this.btnXoaNguoiDung);
            this.panelButtons.Controls.Add(this.btnThemMoi);
            this.panelButtons.Controls.Add(this.btnSua);
            this.panelButtons.Controls.Add(this.btnKhoaMoKhoa);
            this.panelButtons.Controls.Add(this.btnDatLaiMatKhau);
            this.panelButtons.Controls.Add(this.btnDong);
            this.panelButtons.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelButtons.Location = new System.Drawing.Point(0, 547);
            this.panelButtons.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.panelButtons.Name = "panelButtons";
            this.panelButtons.Size = new System.Drawing.Size(1063, 68);
            this.panelButtons.TabIndex = 6;
            // 
            // btnXoaNguoiDung
            // 
            this.btnXoaNguoiDung.BackColor = System.Drawing.Color.Red;
            this.btnXoaNguoiDung.FlatAppearance.BorderSize = 0;
            this.btnXoaNguoiDung.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnXoaNguoiDung.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnXoaNguoiDung.ForeColor = System.Drawing.Color.White;
            this.btnXoaNguoiDung.Location = new System.Drawing.Point(735, 12);
            this.btnXoaNguoiDung.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnXoaNguoiDung.Name = "btnXoaNguoiDung";
            this.btnXoaNguoiDung.Size = new System.Drawing.Size(184, 43);
            this.btnXoaNguoiDung.TabIndex = 5;
            this.btnXoaNguoiDung.Text = "✘ Xóa người dùng";
            this.btnXoaNguoiDung.UseVisualStyleBackColor = false;
            this.btnXoaNguoiDung.Click += new System.EventHandler(this.button1_Click);
            // 
            // btnThemMoi
            // 
            this.btnThemMoi.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(130)))), ((int)(((byte)(70)))));
            this.btnThemMoi.FlatAppearance.BorderSize = 0;
            this.btnThemMoi.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnThemMoi.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnThemMoi.ForeColor = System.Drawing.Color.White;
            this.btnThemMoi.Location = new System.Drawing.Point(13, 12);
            this.btnThemMoi.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnThemMoi.Name = "btnThemMoi";
            this.btnThemMoi.Size = new System.Drawing.Size(153, 43);
            this.btnThemMoi.TabIndex = 0;
            this.btnThemMoi.Text = "➕ Thêm mới";
            this.btnThemMoi.UseVisualStyleBackColor = false;
            this.btnThemMoi.Click += new System.EventHandler(this.btnThemMoi_Click);
            // 
            // btnSua
            // 
            this.btnSua.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(100)))), ((int)(((byte)(180)))));
            this.btnSua.FlatAppearance.BorderSize = 0;
            this.btnSua.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSua.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnSua.ForeColor = System.Drawing.Color.White;
            this.btnSua.Location = new System.Drawing.Point(180, 12);
            this.btnSua.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnSua.Name = "btnSua";
            this.btnSua.Size = new System.Drawing.Size(153, 43);
            this.btnSua.TabIndex = 1;
            this.btnSua.Text = "✏️ Sửa";
            this.btnSua.UseVisualStyleBackColor = false;
            this.btnSua.Click += new System.EventHandler(this.btnSua_Click);
            // 
            // btnKhoaMoKhoa
            // 
            this.btnKhoaMoKhoa.BackColor = System.Drawing.Color.Orange;
            this.btnKhoaMoKhoa.FlatAppearance.BorderSize = 0;
            this.btnKhoaMoKhoa.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnKhoaMoKhoa.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnKhoaMoKhoa.ForeColor = System.Drawing.Color.White;
            this.btnKhoaMoKhoa.Location = new System.Drawing.Point(347, 12);
            this.btnKhoaMoKhoa.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnKhoaMoKhoa.Name = "btnKhoaMoKhoa";
            this.btnKhoaMoKhoa.Size = new System.Drawing.Size(173, 43);
            this.btnKhoaMoKhoa.TabIndex = 2;
            this.btnKhoaMoKhoa.Text = "🔒 Khóa TK";
            this.btnKhoaMoKhoa.UseVisualStyleBackColor = false;
            this.btnKhoaMoKhoa.Click += new System.EventHandler(this.btnKhoaMoKhoa_Click);
            // 
            // btnDatLaiMatKhau
            // 
            this.btnDatLaiMatKhau.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(180)))), ((int)(((byte)(130)))), ((int)(((byte)(0)))));
            this.btnDatLaiMatKhau.FlatAppearance.BorderSize = 0;
            this.btnDatLaiMatKhau.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDatLaiMatKhau.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnDatLaiMatKhau.ForeColor = System.Drawing.Color.White;
            this.btnDatLaiMatKhau.Location = new System.Drawing.Point(533, 12);
            this.btnDatLaiMatKhau.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnDatLaiMatKhau.Name = "btnDatLaiMatKhau";
            this.btnDatLaiMatKhau.Size = new System.Drawing.Size(193, 43);
            this.btnDatLaiMatKhau.TabIndex = 3;
            this.btnDatLaiMatKhau.Text = "🔑 Đặt lại MK";
            this.btnDatLaiMatKhau.UseVisualStyleBackColor = false;
            this.btnDatLaiMatKhau.Click += new System.EventHandler(this.btnDatLaiMatKhau_Click);
            // 
            // btnDong
            // 
            this.btnDong.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(180)))), ((int)(((byte)(180)))), ((int)(((byte)(180)))));
            this.btnDong.FlatAppearance.BorderSize = 0;
            this.btnDong.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDong.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.btnDong.Location = new System.Drawing.Point(927, 12);
            this.btnDong.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnDong.Name = "btnDong";
            this.btnDong.Size = new System.Drawing.Size(120, 43);
            this.btnDong.TabIndex = 4;
            this.btnDong.Text = "Đóng";
            this.btnDong.UseVisualStyleBackColor = false;
            this.btnDong.Click += new System.EventHandler(this.btnDong_Click);
            // 
            // frmQuanLyTaiKhoan
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(1063, 615);
            this.Controls.Add(this.dgvTaiKhoan);
            this.Controls.Add(this.panelSearch);
            this.Controls.Add(this.panelTop);
            this.Controls.Add(this.panelButtons);
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.MinimumSize = new System.Drawing.Size(927, 605);
            this.Name = "frmQuanLyTaiKhoan";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Quản lý tài khoản người dùng";
            this.Load += new System.EventHandler(this.frmQuanLyTaiKhoan_Load);
            this.panelTop.ResumeLayout(false);
            this.panelSearch.ResumeLayout(false);
            this.panelSearch.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvTaiKhoan)).EndInit();
            this.panelButtons.ResumeLayout(false);
            this.ResumeLayout(false);

        }
        #endregion

        private System.Windows.Forms.Panel panelTop;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Panel panelSearch;
        private System.Windows.Forms.Label lblTimKiem;
        private System.Windows.Forms.TextBox txtTimKiem;
        private System.Windows.Forms.Button btnTimKiem;
        private System.Windows.Forms.Button btnLamMoi;
        private System.Windows.Forms.Label lblSoLuong;
        private System.Windows.Forms.DataGridView dgvTaiKhoan;
        private System.Windows.Forms.Panel panelButtons;
        private System.Windows.Forms.Button btnThemMoi;
        private System.Windows.Forms.Button btnSua;
        private System.Windows.Forms.Button btnKhoaMoKhoa;
        private System.Windows.Forms.Button btnDatLaiMatKhau;
        private System.Windows.Forms.Button btnDong;
        private System.Windows.Forms.Button btnXoaNguoiDung;
    }
}
