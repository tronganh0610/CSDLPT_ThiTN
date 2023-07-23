using DevExpress.XtraEditors;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CSDLPT
{
    public partial class FormDangNhap : Form
    {
        private SqlConnection connPublisher = new SqlConnection();
        private void layDanhSachPhanManh(String cmd)
        {
            if (connPublisher.State == ConnectionState.Closed)
            {
                connPublisher.Open();
            }
            DataTable dt = new DataTable();
            // adapter dùng để đưa dữ liệu từ view sang database
            SqlDataAdapter da = new SqlDataAdapter(cmd, connPublisher);
            // dùng adapter thì mới đổ vào data table được
            da.Fill(dt);


            connPublisher.Close();
            Program.bindingSource.DataSource = dt;


            cmbCoSo.DataSource = Program.bindingSource;
            cmbCoSo.DisplayMember = "TENCS";
            cmbCoSo.ValueMember = "TENSERVER";
        }


        public FormDangNhap()
        {
            InitializeComponent();
        }
        private Form CheckExists(Type ftype)
        {
            foreach (Form f in this.MdiChildren)
                if (f.GetType() == ftype)
                    return f;
            return null;
        }
        private int KetNoiDatabaseGoc()
        {
            if (connPublisher != null && connPublisher.State == ConnectionState.Open)
                connPublisher.Close();
            try
            {
                connPublisher.ConnectionString = Program.connstrPublisher;
                connPublisher.Open();
                return 1;
            }

            catch (Exception e)
            {
                MessageBox.Show("Lỗi kết nối cơ sở dữ liệu.\nBạn xem lại user name và password.\n " + e.Message, "", MessageBoxButtons.OK);
                return 0;
            }
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

       

        private void panelControl1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void FormDangNhap_Load(object sender, EventArgs e)
        {
            // đặt sẵn mật khẩu để đỡ nhập lại nhiều lần
            txtTenDangNhap.Text = "TH101";// 
            txtMatKhau.Text = "12";
            if (KetNoiDatabaseGoc() == 0)
                return;
            //Lấy 2 cái đầu tiên của danh sách
            layDanhSachPhanManh("SELECT  * FROM View_DS_COSO");
            checkGV.Checked = true;

            
        }
        private void button1_Click(object sender, EventArgs e)
        {
            /* Step 1*/
            if (txtTenDangNhap.Text.Trim() == "" || txtMatKhau.Text.Trim() == "")
            {
                MessageBox.Show("Tài khoản & mật khẩu không thể bỏ trống", "Thông Báo", MessageBoxButtons.OK);
                return;
            }
            /* Step 2*/
            Program.loginName = txtTenDangNhap.Text.Trim();
            Program.loginPassword = txtMatKhau.Text.Trim();
            if (Program.KetNoi() == 0)
                return;
            /* Step 3*/
            Program.brand = cmbCoSo.SelectedIndex;
            Program.currentLogin = Program.loginName;
            Program.currentPassword = Program.loginPassword;


            /* Step 4*/
            String statement = "EXEC sp_LayTTDangNhap '" + Program.loginName + "'";// exec sp_DangNhap 'TP'
            Program.myReader = Program.ExecSqlDataReader(statement);
            if (Program.myReader == null)
                return;
            // đọc một dòng của myReader - điều này là hiển nhiên vì kết quả chỉ có 1 dùng duy nhất
            Program.myReader.Read();


            /* Step 5*/
            Program.userName = Program.myReader.GetString(0);// lấy userName
            if (Convert.IsDBNull(Program.userName))
            {
                MessageBox.Show("Tài khoản này không có quyền truy cập \n Hãy thử tài khoản khác", "Thông Báo", MessageBoxButtons.OK);
            }



            Program.staff = Program.myReader.GetString(1);
            Program.role = Program.myReader.GetString(2);

            Program.myReader.Close();
            Program.conn.Close();


           
            Program.formChinh.UserName.Text = "UserName: " + Program.userName ;
            



            /* Step 6*/
            
                this.Visible = false;
                Program.formChinh.Show();
                

        }

        private void button2_Click(object sender, EventArgs e)
        {
            Form f = this.CheckExists(typeof(FormDangNhap));
            if (f != null)
            {
                f.Activate();
            }
            else
            {
                
                this.Close();
            }
        }

        private void cmbCoSo_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                Program.serverName = cmbCoSo.SelectedValue.ToString();
                //Console.WriteLine(cmbCoSo.SelectedValue.ToString());
            }
            catch (Exception)
            {

            }
        }
    }
    
}
