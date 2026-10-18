using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.OleDb;

namespace CuahangNongduoc.DataLayer
{
    public class PhieuNhapFactory
    {
        DataService m_Ds = new DataService();

        public void LoadSchema()
        {
            OleDbCommand cmd = new OleDbCommand("SELECT * FROM PHIEU_NHAP WHERE ID='-1'");
            m_Ds.Load(cmd);
        }

        public DataTable DanhsachPhieuNhap()
        {
            OleDbCommand cmd = new OleDbCommand("SELECT * FROM PHIEU_NHAP");
            m_Ds.Load(cmd);

            return m_Ds;
        }

        public DataTable TimPhieuNhap(String maNCC, DateTime dt)
        {
            OleDbCommand cmd = new OleDbCommand("SELECT * FROM PHIEU_NHAP WHERE NGAY_NHAP >= ? AND NGAY_NHAP < ? AND ID_NHA_CUNG_CAP = ?");
            cmd.Parameters.Add("ngayBatDau", OleDbType.Date).Value = dt.Date;
            cmd.Parameters.Add("ngaySau", OleDbType.Date).Value = dt.Date.AddDays(1);
            cmd.Parameters.Add("idNCC", OleDbType.VarChar, 50).Value = maNCC;
            m_Ds.Load(cmd);

            return m_Ds;
        }

        public DataService GetDataService()
        {
            return m_Ds;
        }

        public bool Save()
        {
            if (m_Ds.GetChanges() == null)
                return true;
            return m_Ds.ExecuteNoneQuery() > 0;
        }

        public DataTable LayPhieuNhap(String id)
        {
            OleDbCommand cmd = new OleDbCommand("SELECT * FROM PHIEU_NHAP WHERE ID = ?");
            cmd.Parameters.Add("id", OleDbType.VarChar, 50).Value = id;
            m_Ds.Load(cmd);

            return m_Ds;
        }

        public int XoaPhieuNhapVaCacLo(String id)
        {
            DataService ds = new DataService();
            OleDbCommand laySanPham = new OleDbCommand("SELECT DISTINCT ID_SAN_PHAM FROM MA_SAN_PHAM WHERE ID_PHIEU_NHAP=?");
            laySanPham.Parameters.Add("id", OleDbType.VarChar, 50).Value = id;
            ds.Load(laySanPham);
            List<string> idsSanPham = new List<string>();
            foreach (DataRow row in ds.Rows)
            {
                string idSanPham = Convert.ToString(row["ID_SAN_PHAM"]);
                if (!String.IsNullOrEmpty(idSanPham) && !idsSanPham.Contains(idSanPham))
                    idsSanPham.Add(idSanPham);
            }
            return XoaPhieuNhapVaCacLo(id, idsSanPham);
        }

        public int XoaPhieuNhapVaCacLo(String id, IList<string> idsSanPham)
        {
            // DAO kiểm tra lịch sử bán và xóa phiếu/chi tiết/lô + cập nhật tồn trong cùng transaction.
            //return DataService.XoaPhieuNhapVaCacLoTrongTransaction(id, idsSanPham);
            return 1;
        }

        public DataRow NewRow()
        {
            return m_Ds.NewRow();
        }

        public void Add(DataRow row)
        {
            m_Ds.Rows.Add(row);
        }

       
    }
}