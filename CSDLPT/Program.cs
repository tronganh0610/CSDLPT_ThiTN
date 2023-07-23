using System;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using System.Data;
using System.Data.SqlClient;
using System.Collections;

namespace CSDLPT
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        public static SqlConnection conn = new SqlConnection();
        public static String connstr;
        public static String connstrPublisher ="Data Source=MSI\\MAYCHU;Initial Catalog= TN ;Persist Security Info=True;User ID=sa;Password=12";
                                                         // MSI\\MAYCHU
        public static SqlDataReader myReader;
        public static String serverName = "";//servername
        public static String serverNameLeft = "";
        public static String userName = "";//username

        public static String loginName = "";//mlogin
        public static String loginPassword = "";//password
        public static Boolean dangXuat = false;

        public static String database = "TN";
        public static String remoteLogin = "htkn";//remotelogin
        public static String remotePassword = "12";//remotepassword

        public static String currentLogin = "";//mloginDN
        public static String currentPassword = "";//passwordDN
        public static String role = "";// mGroup
        public static String staff = "";//mHoten
      
        public static int brand = 0;//mCoSo

        public static string MaLopSV = "";
        public static string TenLopSV = "";

        public static FormDangNhap formDangNhap;
        public static FormChinh formChinh;
       


        /*bidSou: BindingSource -> liên kết dữ liệu từ bảng dữ liệu vào chương trình*/
        public static BindingSource bindingSource = new BindingSource();//bds_dspm

        public static BindingSource bds_dspm = new BindingSource();  // giữ bdsPM khi đăng nhập
                                                                     //public static formMain frmChinh;
        public static int KetNoi()
        {
            if (Program.conn != null && Program.conn.State == ConnectionState.Open)
                Program.conn.Close();
            try
            {
                Program.connstr = "Data Source=" + Program.serverName + ";Initial Catalog=" +
                       Program.database + ";User ID=" +
                       Program.loginName + ";password=" + Program.loginPassword;
                Program.conn.ConnectionString = Program.connstr;

                Program.conn.Open();
                return 1;
            }

            catch (Exception e)
            {
                MessageBox.Show("Lỗi kết nối cơ sở dữ liệu.\nXem lại tài khoản và mật khẩu.\n " + e.Message, "", MessageBoxButtons.OK);
                //Console.WriteLine(e.Message);
                return 0;
            }
        }
        public static SqlDataReader ExecSqlDataReader(String strLenh)
        {
            SqlDataReader myreader;
            SqlCommand sqlcmd = new SqlCommand(strLenh, Program.conn);
            sqlcmd.CommandType = CommandType.Text;
            sqlcmd.CommandTimeout = 600;
            if (Program.conn.State == ConnectionState.Closed)
                Program.conn.Open();
            try
            {
                myreader = sqlcmd.ExecuteReader();

                return myreader;
            }
            catch (SqlException ex)
            {
                Program.conn.Close();
                MessageBox.Show(ex.Message);
                return null;
            }
        }
        public static SqlDataReader ExecSqlDataReader(String strLenh, Hashtable paras)
        {
            SqlDataReader myreader = null;
            SqlCommand sqlcmd = new SqlCommand(strLenh, Program.conn);
            sqlcmd.CommandType = CommandType.Text;

            foreach (DictionaryEntry s in paras)
            {
                sqlcmd.Parameters.AddWithValue(s.Key.ToString(), s.Value);
            }
            if (Program.conn.State == ConnectionState.Closed) Program.conn.Open();
            try
            {
                myreader = sqlcmd.ExecuteReader();
                return myreader;
            }
            catch (SqlException)
            {
                Program.conn.Close();
                //MessageBox.Show(ex.Message);
                return null;
            }
        }
        public static DataTable ExecSqlDataTable(String cmd)
        {
            DataTable dt = new DataTable();
            if (Program.conn.State == ConnectionState.Closed)
                Program.conn.Open();
            SqlDataAdapter da = new SqlDataAdapter(cmd, conn);
            da.Fill(dt);
            conn.Close();
            return dt;
        }
        public static DataTable ExecSqlDataTable(String cmd, Hashtable paras)
        {
            DataTable dt = new DataTable();
            if (Program.conn.State == ConnectionState.Closed) Program.conn.Open();
            SqlDataAdapter da = new SqlDataAdapter(cmd, conn);
            // da.SelectCommand.Parameters.AddWithValue;
            foreach (DictionaryEntry s in paras)
            {
                da.SelectCommand.Parameters.AddWithValue(s.Key.ToString(), s.Value);
            }
            da.Fill(dt);
            conn.Close();
            return dt;
        }
        public static int ExecSqlNonQuery(String strlenh)
        {
            SqlCommand Sqlcmd = new SqlCommand(strlenh, conn);
            Sqlcmd.CommandType = CommandType.Text;
            Sqlcmd.CommandTimeout = 600;// 10 phut 
            if (conn.State == ConnectionState.Closed) conn.Open();
            try
            {
                Sqlcmd.ExecuteNonQuery();
                conn.Close();
                return 0;
            }
            catch (SqlException ex)
            {

                if (ex.Message.Contains("Cannot alter the login '" + Program.loginName.Trim() + "', because it does not exist or you do not have permission."))
                    MessageBox.Show("Mật khẩu cũ không đúng");
                else MessageBox.Show(ex.Message);
                conn.Close();
                return ex.State; // trang thai lỗi gởi từ RAISERROR trong SQL Server qua
            }
        }
        public static int ExecSqlNonQuery(String strlenh, Hashtable paras)
        {
            SqlCommand Sqlcmd = new SqlCommand(strlenh, conn);
            Sqlcmd.CommandType = CommandType.Text;
            Sqlcmd.CommandTimeout = 600;// 10 phut 
            foreach (DictionaryEntry s in paras)
            {
                Sqlcmd.Parameters.AddWithValue(s.Key.ToString(), s.Value);
            }
            if (conn.State == ConnectionState.Closed) conn.Open();
            try
            {
                Sqlcmd.ExecuteNonQuery(); conn.Close();
                return 0;
            }
            catch (SqlException ex)
            {
                //if (ex.Message.Contains("Error converting data type varchar to int"))
                //    MessageBox.Show("Bạn format Cell lại cột \"Ngày Thi\" qua kiểu Number hoặc mở File Excel.");
                //else 
                MessageBox.Show(ex.Message + strlenh);
                conn.Close();
                return ex.State; // trang thai lỗi gởi từ RAISERROR trong SQL Server qua
            }
        }
        public static object ExecSqlScalar(string query)
        {
            using (SqlConnection connection = new SqlConnection(connstr))
            {
                SqlCommand command = new SqlCommand(query, connection);
                command.CommandType = CommandType.Text;
                connection.Open();
                object result = command.ExecuteScalar();
                return result;
            }
        }


        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            formDangNhap = new FormDangNhap();
            Application.Run(formDangNhap);
        }
    }
}
