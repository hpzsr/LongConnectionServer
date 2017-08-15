using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Long_Connection
{
    public partial class Form1 : Form
    {
        Socket m_socket;
        IPAddress m_ipAddress;
        int m_ipPort;

        bool m_isStart = false;

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            m_ipAddress = IPAddress.Parse(textBox_ipAddress.Text);
            m_ipPort = Convert.ToInt32(textBox_ipPort.Text);

            //MySqlUtil.getInstance().openDatabase("game");

            m_isStart = true;

            Thread t1 = new Thread(new ThreadStart(ListenClient));
            t1.Start();

            ((Button)sender).Enabled = false;
            button_stop.Enabled = true;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            m_isStart = false;
            m_socket.Close();
            //MySqlUtil.getInstance().closeDatabase();

            ((Button)sender).Enabled = false;
            button_start.Enabled = true;
        }

        // 监听客户端请求
        void ListenClient()
        {
            try
            {
                IPEndPoint iPEndPoint = new IPEndPoint(m_ipAddress, m_ipPort);
                m_socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                m_socket.Bind(iPEndPoint);
                m_socket.Listen(400);

                while (m_isStart)
                {
                    Socket socketback = m_socket.Accept();

                    // 另开一个线程处理客户端的请求，防止阻塞
                    Thread t1 = new Thread(new ParameterizedThreadStart(DoTaskClient));
                    t1.Start(socketback);
                }
            }
            catch (SocketException ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        // 处理客户端请求
        void DoTaskClient(object socket)
        {
            Debug.WriteLine("获取客户端发起的请求");

            Socket socketback = (Socket)socket;

            // 接收消息
            receive(socketback);

            //socketback.Close();
        }

        public void receive(Socket socketback)
        {
            byte[] rece = new byte[1024];
            while (true)
            {
                if (socketback != null && socketback.Connected)
                {
                    int recelong = socketback.Receive(rece, rece.Length, 0);
                    string reces = Encoding.ASCII.GetString(rece, 0, recelong);
                    // 解密
                    //reces = _3DES.DESDecrypst(reces);

                    
                    Debug.WriteLine("客户端请求数据：" + reces);

                    string backData = "hello client";
                    Debug.WriteLine("返回给客户端数据：" + backData);

                    // 发送消息
                    sendmessage(socketback, backData);
                }
                else
                {
                    Debug.WriteLine("连接释放");
                }
            }
        }

        public void sendmessage(Socket socketback, string str)
        {
            // 加密
            //str = _3DES.DESEncrypt(str);

            byte[] bytes = new byte[1024];
            bytes = Encoding.ASCII.GetBytes(str);
            socketback.Send(bytes);
        }

        void destroy()
        {
            m_socket.Close();
        }
    }
}
