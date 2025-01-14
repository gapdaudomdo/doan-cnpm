﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using BLL;
using DTO;
using GUI.XuLy;
namespace GUI
{
    public partial class frmBanSanPham : Form
    {
        QL_CUAHANGLINHKIENMAYTINHDataContext qllk = new QL_CUAHANGLINHKIENMAYTINHDataContext();
        BLLHoaDon bllhoadon = new BLLHoaDon();
        BLLGioHang bllgio = new BLLGioHang();
        BLLKhachHang bllkh = new BLLKhachHang();
        BLLDangNhap blldn = new BLLDangNhap();
        BLLSeri bllsr = new BLLSeri();
        BLLBaoHanh bllbh = new BLLBaoHanh();

        Helper hp = new Helper();
        public static string taikhoan;
        public static int mahd;
        public static int tongtien;
        public static int manv;
        public static string ten;
        public static string tennv;
        public static int chonmanv;
        public frmBanSanPham()
        {
            InitializeComponent();
        }

        private void frmBanSanPham_Load(object sender, EventArgs e)
        {
            Refresh();
            dgv_HoaDon.DataSource = bllhoadon.LoadHoaDonNV(int.Parse(frmTrangChuNhanVien.manv));
            loaddata(frmQuanLySP.lstsp);
            if (dgv_Chitiethoadon.Rows.Count >= 0)
            {
                dgv_Chitiethoadon.Columns[5].Visible = false;
            }
            txt_MaNV.Text = frmTrangChuNhanVien.manv;
            comboBox1.DataSource = bllkh.loadmakh();
            comboBox1.DisplayMember = "MaKH";
            comboBox1.ValueMember = "MaKH";
            comboBox1.SelectedIndex = 0;

            comboBox1.Text = frmChonKhachHang.ma.ToString();

            guna2Button3.Enabled = true;
        }

        private void dgv_HoaDon_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = this.dgv_HoaDon.Rows[e.RowIndex];
                txt_MaHoaDon.Text = row.Cells[0].Value.ToString();
                comboBox1.Text = row.Cells[1].Value.ToString();

                //txt_TongTien.Text = row.Cells[3].Value.ToString();
                dateTimePicker1.Text = dgv_HoaDon.Rows[e.RowIndex].Cells[2].Value.ToString();
                txt_MaNV.Text = row.Cells[3].Value.ToString();

                btn_Xuat.Enabled = true;
                toolStripButton1.Enabled = true;

                
                dgv_chitiet.DataSource = bllgio.loadChiTietHoaDon(int.Parse(row.Cells[0].Value.ToString()));
                int tongtien=0;
                for(int i=0;i< dgv_chitiet.RowCount ; i++)
                {
                    tongtien +=int.Parse(dgv_chitiet.Rows[i].Cells[4].Value.ToString());
                   
                }
                txt_TongTien.Text = tongtien.ToString();
            }
        }

        private void btn_ThemHD_Click(object sender, EventArgs e)
        {
            
            loaddata(frmQuanLySP.lstsp);
            int a = 0;
            if (comboBox1.Text == a.ToString())
            {
                MessageBox.Show("Vui lòng chọn khách hàng");
                return;
            }
            QuanLyHoaDon cthdsp = new QuanLyHoaDon()
                {
                    MaKH = int.Parse(comboBox1.Text),
                    NgayLapHoaDon = dateTimePicker1.Value,
                    MaNhanVien = int.Parse(txt_MaNV.Text)
                };

            if (bllhoadon.postHD(cthdsp))
                {
                    dgv_HoaDon.DataSource = bllhoadon.LoadHoaDonNV(int.Parse(txt_MaNV.Text));
                    int so = dgv_HoaDon.RowCount;
                    dgv_HoaDon.CurrentCell = dgv_HoaDon[1, so-1];
                    txt_MaHoaDon.Text = dgv_HoaDon.CurrentRow.Cells[0].Value.ToString();
                    for (int x = 0; x < dgv_Chitiethoadon.Rows.Count; x++)
                    {
                        ThemCTHD cthd = new ThemCTHD()
                        {
                            MaHoaDon = int.Parse(txt_MaHoaDon.Text),
                            MaSanPham = int.Parse(dgv_Chitiethoadon.Rows[a].Cells[6].Value.ToString()),
                            giaban = int.Parse(dgv_Chitiethoadon.Rows[a].Cells[3].Value.ToString()),
                            soluong = int.Parse(dgv_Chitiethoadon.Rows[a].Cells[4].Value.ToString()),
                            TongTien = int.Parse(dgv_Chitiethoadon.Rows[a].Cells[3].Value.ToString()) * int.Parse(dgv_Chitiethoadon.Rows[a].Cells[4].Value.ToString()),

                        };
                        
                        if (bllgio.postGioHangCTHD(cthd))
                        {
                            for (int i = 0; i < int.Parse(dgv_Chitiethoadon.Rows[a].Cells[4].Value.ToString()); i++)
                            {
                                SeriHD sr = new SeriHD()
                                {
                                    Seri = bllsr.loadseritheoma(int.Parse(dgv_Chitiethoadon.Rows[a].Cells[6].Value.ToString())),
                                    MaHoaDon = int.Parse(txt_MaHoaDon.Text),
                                };
                                bllsr.postSP(sr);
                            }
                        }
                        else
                        {
                            MessageBox.Show("Mua hàng không thành công");
                            break;
                        }
                        a++;
                    }
                    MessageBox.Show("Mua hàng thành công");
                }
            frmQuanLySP.lstsp.Clear();
        }

        private void btn_XoaHD_Click(object sender, EventArgs e)
        {
            if (bllgio.deleteGioHangCTHD(int.Parse(txt_MaHoaDon.Text)))
               
                {
                    bllgio.deleteGioHang(int.Parse(txt_MaHoaDon.Text));
                    MessageBox.Show("Xóa thành công");
                    dgv_HoaDon.DataSource = bllhoadon.LoadHoaDon();
                }
                else
                {
                    MessageBox.Show("Xóa thất bại");
                    return;
                }

        }

        private void btn_SuaHD_Click(object sender, EventArgs e)
        {
            QuanLyHoaDon kh = new QuanLyHoaDon()
            {

                //MaHoaDon = int.Parse(txt_MaNV.Text),
                MaKH = int.Parse(comboBox1.Text),
                NgayLapHoaDon = dateTimePicker1.Value,
                MaNhanVien = int.Parse(txt_MaNV.Text) 
            };

            if (bllhoadon.putHD(kh, int.Parse(txt_MaHoaDon.Text)))
            {
                MessageBox.Show(" Sửa thành công");
                dgv_HoaDon.DataSource = bllhoadon.LoadHoaDonNV(int.Parse(txt_MaNV.Text));
            }
            else
            {
                MessageBox.Show("Sửa thất bại");
                return;
            }

        }

        public static int kq;
        public Image GetImg(string direct, int w, int h)
        {
            Image i = Image.FromFile(direct);
            return (Image)(new Bitmap(i, new Size(w, h)));
        }
        private void dgv_Chitiethoadon_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                //kq = int.Parse(dgv_Chitiethoadon.CurrentRow.Cells[3].Value.ToString()) * int.Parse(dgv_Chitiethoadon.CurrentRow.Cells[4].Value.ToString());
                //txt_TongTien.Text = kq.ToString();
                string direct = hp.Directory() + dgv_Chitiethoadon.CurrentRow.Cells[5].Value.ToString();
                pictureBox1.Image = GetImg(direct, pictureBox1.Width, pictureBox1.Height);

                guna2Button1.Enabled = true;
            }
        }
        public void loaddata(BindingList<ThemSanPham> loadsp)
        {
            dgv_Chitiethoadon.DataSource = loadsp;

            dgv_Chitiethoadon.Columns[0].ReadOnly = true;
            dgv_Chitiethoadon.Columns[1].ReadOnly = true;
            dgv_Chitiethoadon.Columns[2].ReadOnly = true;
            dgv_Chitiethoadon.Columns[3].ReadOnly = true;
            dgv_Chitiethoadon.Columns[4].ReadOnly = true;
            dgv_Chitiethoadon.Columns[5].ReadOnly = true;
            dgv_Chitiethoadon.Columns[6].ReadOnly = true;
        }
        public void loadkh(BindingList<ThemSanPham> loadsp)
        {
            comboBox1.DataSource = loadsp;
        }
        private void button1_Click_1(object sender, EventArgs e)
        {
            

            
        }

        private void btn_Xuat_Click(object sender, EventArgs e)
        {
            ten = blldn.loadtentheomakh(comboBox1.Text);
            tennv = blldn.loadtentheoma(txt_MaNV.Text);

            dataExcel.DataSource = bllgio.loadBieuMauGioHangAPI(int.Parse(comboBox1.Text),int.Parse(txt_MaHoaDon.Text));
            taikhoan = comboBox1.Text;
            mahd = int.Parse(dgv_HoaDon.CurrentRow.Cells[0].Value.ToString());
            ExcelExportNV ex = new ExcelExportNV();
            if (dgv_HoaDon.Rows.Count == 0)
            {
                MessageBox.Show("khong co du lieu de xuat");
                return;
            }
            int a = 0;
            for (int x = 0; x < dataExcel.Rows.Count; x++)
            {
                tongtien += int.Parse(dataExcel.Rows[a].Cells[1].Value.ToString()) * int.Parse(dataExcel.Rows[a].Cells[2].Value.ToString());
                a++;
            }
            manv = int.Parse(txt_MaNV.Text);
            List<View_BieuMauGio> pListKhoa = new List<View_BieuMauGio>();

                foreach (DataGridViewRow item in dataExcel.Rows)
                {
                    View_BieuMauGio i = new View_BieuMauGio();
                    i.TenSanPham = item.Cells[0].Value.ToString();
                    i.soluong = int.Parse(item.Cells[1].Value.ToString());
                    i.giaban = int.Parse(item.Cells[2].Value.ToString());
                    i.NgayLapHoaDon = DateTime.Parse(item.Cells[3].Value.ToString());
                    i.TongTien = int.Parse(item.Cells[5].Value.ToString());

                    pListKhoa.Add(i);
                }


            string path = string.Empty;

            ex.ExportKhoa(pListKhoa, ref path, false);

            DialogResult r = MessageBox.Show("ban co muon mo file khong", "thong bao", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (!string.IsNullOrEmpty(path) && r == DialogResult.Yes)
            {
                System.Diagnostics.Process.Start(path);
            }

            btn_Xuat.Enabled = false;
            toolStripButton1.Enabled = false;
        }

        private void guna2Button3_Click(object sender, EventArgs e)
        {
            Form fsf = Application.OpenForms["frmQuanLySP"];

            if (fsf != null)
            {
                return;
            }
            else
            {
                frmQuanLySP frm = new frmQuanLySP();
                frm.Show();
                frm.TopMost = true;
                frm.toolStripButton1.Visible = true;
                frm.txt_GiaBan.Enabled = false;
                frm.txt_HangSX.Enabled = false;
                frm.txt_Image.Enabled = false;
                frm.txt_LoaiSP.Enabled = false;
                frm.txt_MaSP.Enabled = false;
                frm.txt_TenSP.Enabled = false;
                frm.txt_TonKho.Enabled = false;
                frm.txtHSD.Enabled = false;
                frm.btn_Sua.Visible = false;
                frm.btn_Them.Visible = false;
                frm.btn_Xoa.Visible = false;
                frm.btn.Enabled = false;
            }
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            ten = blldn.loadtentheomakh(comboBox1.Text);
            tennv = blldn.loadtentheoma(txt_MaNV.Text);

            dataExcel.DataSource = bllbh.loadbieumau(int.Parse(txt_MaHoaDon.Text));
            taikhoan = comboBox1.Text;
            mahd = int.Parse(dgv_HoaDon.CurrentRow.Cells[0].Value.ToString());
            ExcelExportBH ex = new ExcelExportBH();
            if (dgv_HoaDon.Rows.Count == 0)
            {
                MessageBox.Show("khong co du lieu de xuat");
                return;
            }
            int a = 0;
            for (int x = 0; x < dataExcel.Rows.Count; x++)
            {
                tongtien += 1 * int.Parse(dataExcel.Rows[a].Cells[2].Value.ToString());
                a++;
            }
            manv = int.Parse(txt_MaNV.Text);
            List<View_BieuMauBaoHanh> pListKhoa = new List<View_BieuMauBaoHanh>();
            int b = 0;
                for (int x = 0; x < dataExcel.Rows.Count; x++)
                {

                    View_BieuMauBaoHanh i = new View_BieuMauBaoHanh();
                    
                    if (b <= dataExcel.Rows.Count)
                    {
                        i.TenSanPham = dataExcel.Rows[b].Cells[0].Value.ToString();
                        i.soluong = 1;
                        i.giaban = int.Parse(dataExcel.Rows[b].Cells[2].Value.ToString());
                        i.NgayLapHoaDon = DateTime.Parse((dataExcel.Rows[b].Cells[3].Value.ToString()));
                        i.TongTien = int.Parse(dataExcel.Rows[b].Cells[2].Value.ToString());
                        i.Seri = dataExcel.Rows[b].Cells[7].Value.ToString();
                        pListKhoa.Add(i);
                        //if (dataExcel.Rows[b - 1].Cells[0].Value.ToString() == dataExcel.Rows[b].Cells[0].Value.ToString())
                        //{
                        //    i.TenSanPham = null;
                        //    i.soluong = null;
                        //    i.giaban = null;
                        //    i.NgayLapHoaDon = null;
                        //    i.TongTien = null;
                        //    i.Seri = dataExcel.Rows[b].Cells[7].Value.ToString();
                        //    pListKhoa.Add(i);
                        //}
                    } b++;
                    
                }
                

            string path = string.Empty;

            ex.ExportKhoa(pListKhoa, ref path, false);

            DialogResult r = MessageBox.Show("ban co muon mo file khong", "thong bao", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (!string.IsNullOrEmpty(path) && r == DialogResult.Yes)
            {
                System.Diagnostics.Process.Start(path);
            }

            btn_Xuat.Enabled = false;
            toolStripButton1.Enabled = false;
            tongtien = 0;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            frmChonKhachHang frm = new frmChonKhachHang(this);
            frm.ShowDialog();
        }

        private void guna2Button1_Click(object sender, EventArgs e)
        {
            frmQuanLySP.lstsp.RemoveAt(dgv_Chitiethoadon.CurrentCell.RowIndex);
            guna2Button1.Enabled = false;
        }
    }
}
