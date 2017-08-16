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
        Socket m_socket = null;
        //IPAddress m_ipAddress = IPAddress.Parse("222.73.65.206");
        //int m_ipPort = Convert.ToInt32("10103");
        IPAddress m_ipAddress = IPAddress.Parse("127.0.0.1");
        int m_ipPort = Convert.ToInt32("10008");

        Socket m_socketClient = null;

        bool m_isStart = false;

        public Form1()
        {
            InitializeComponent();

            Control.CheckForIllegalCrossThreadCalls = false;

            this.button_fasong.Enabled = false;
            this.button_stop.Enabled = false;
        }

        // 启动监听
        private void button1_Click(object sender, EventArgs e)
        {
            m_isStart = true;

            Thread t1 = new Thread(new ThreadStart(ListenClient));
            t1.Start();

            this.button_start.Enabled = false;
            this.button_stop.Enabled = true;
            this.button_fasong.Enabled = true;
        }

        // 停止服务
        private void button2_Click(object sender, EventArgs e)
        {
            Debug.WriteLine("服务端关闭");

            m_isStart = false;

            if (m_socket != null)
            {
                m_socket.Close();
            }

            if (m_socketClient != null)
            {
                m_socketClient.Close();
            }

            this.button_start.Enabled = true;
            this.button_stop.Enabled = false;
            this.button_fasong.Enabled = false;
        }

        // 发送消息
        private void button1_Click_1(object sender, EventArgs e)
        {
            string backData = this.textBox_send.Text;
            this.listBox_chat.Items.Add("        " + backData);

            Debug.WriteLine("返回给客户端数据：" + backData);

            // 发送消息
            sendmessage(m_socketClient, backData);
        }

        // 监听客户端请求
        void ListenClient()
        {
            Debug.WriteLine("启动成功，开始监听");

            try
            {
                IPEndPoint iPEndPoint = new IPEndPoint(m_ipAddress, m_ipPort);
                m_socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                m_socket.Bind(iPEndPoint);
                m_socket.Listen(400);

                while (m_isStart)
                {
                    try
                    {
                        Socket socketback = m_socket.Accept();
                        // 另开一个线程处理客户端的请求，防止阻塞
                        Thread t1 = new Thread(new ParameterizedThreadStart(DoTaskClient));
                        t1.Start(socketback);
                    }
                    catch (SocketException ex)
                    {
                        Debug.WriteLine("与客户端连接断开");
                        Debug.WriteLine("错误日志：" + ex.Message);
                    }
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

            m_socketClient = (Socket)socket;

            // 接收消息
            receive(m_socketClient);
        }

        public void receive(Socket socketback)
        {
            byte[] rece = new byte[1024];
            while (m_isStart)
            {
                if (socketback != null && socketback.Connected)
                {
                    try
                    {
                        int recelong = socketback.Receive(rece, rece.Length, 0);
                        string reces = Encoding.UTF8.GetString(rece, 0, recelong);
                        // 解密
                        //reces = _3DES.DESDecrypst(reces);


                        Debug.WriteLine("客户端请求数据：" + reces);

                        this.listBox_chat.Items.Add(reces);
                    }
                    catch (SocketException ex)
                    {
                        Debug.WriteLine("与客户端连接断开");
                        Debug.WriteLine("错误日志：" + ex.Message);
                        return;
                    }
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

            try
            {
                if (socketback != null)
                {
                    byte[] bytes = new byte[1024];
                    bytes = Encoding.UTF8.GetBytes(str);
                    socketback.Send(bytes);
                }
                else
                {
                    Debug.WriteLine("错误：socketClient为空");
                }
            }
            catch (SocketException ex)
            {
                Debug.WriteLine("与客户端连接断开");
                Debug.WriteLine("错误日志：" + ex.Message);
            }
        }

        void destroy()
        {
            m_socket.Close();
        }
    }
}
