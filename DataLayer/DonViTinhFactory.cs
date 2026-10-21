using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.OleDb;

namespace CuahangNongduoc.DataLayer
{
    public class DonViTinhFactory
    {
        DataService m_Ds = new DataService();

        public DataTable DanhsachDVT()
        {
            OleDbCommand cmd = new OleDbCommand("SELECT * FROM DON_VI_TINH");
            m_Ds.Load(cmd);

            return m_Ds;
        }


        public DataTable LayDVT(int id)
        {
            OleDbCommand cmd = new OleDbCommand("SELECT * FROM DON_VI_TINH WHERE ID = @id");
            cmd.Parameters.Add("id", OleDbType.Integer).Value = id;
            m_Ds.Load(cmd);
            return m_Ds;
        }
        public bool Save()
        {
            return m_Ds.ExecuteNoneQuery() > 0;
        }

        /// <summary>
        /// Hoan nguyen toan bo thay doi chua commit trong DataTable ve trang thai ban dau.
        /// </summary>
        public void RejectChanges()
        {
            m_Ds.RejectChanges();
        }
    }
}
