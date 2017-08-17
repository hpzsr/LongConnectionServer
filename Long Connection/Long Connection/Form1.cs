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

        //Socket m_socketClient = null;
        List<ClientSocket> m_clientSocketList = new List<ClientSocket>();

        bool m_isStart = false;
        int m_clientCount = 0;

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

            for (int i = 0; i < m_clientSocketList.Count; i++)
            {
                if (m_clientSocketList[i].m_clientSocket != null)
                {
                    m_clientSocketList[i].m_clientSocket.Close();
                }
            }

            m_clientSocketList.Clear();
            this.label_peopleNum.Text = "人数:" + m_clientSocketList.Count.ToString();

            this.button_start.Enabled = true;
            this.button_stop.Enabled = false;
            this.button_fasong.Enabled = false;
        }

        // 发送消息
        private void button1_Click_1(object sender, EventArgs e)
        {
            if (m_clientSocketList.Count <= 0)
            {
                Debug.WriteLine("现在没有客户端连接");

                return;
            }

            string backData = this.textBox_send.Text;
            this.listBox_chat.Items.Add("        " + backData);

            Debug.WriteLine("返回给客户端数据：" + backData);

            // 发送消息
            for (int i = 0; i < m_clientSocketList.Count; i++)
            {
                sendmessage(m_clientSocketList[i], backData);
            }
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

                        ClientSocket clientSocket = new ClientSocket((++m_clientCount).ToString(), socketback);
                        m_clientSocketList.Add(clientSocket);
                        this.label_peopleNum.Text = "人数:" + m_clientSocketList.Count.ToString();

                        // 另开一个线程处理客户端的请求，防止阻塞
                        Thread t1 = new Thread(new ParameterizedThreadStart(DoTaskClient));
                        t1.Start(clientSocket);
                    }
                    catch (SocketException ex)
                    {
                        Debug.WriteLine("Socket关闭");
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
        void DoTaskClient(object clientSocket)
        {
            Debug.WriteLine("获取客户端发起的请求");

            // 接收消息
            receive((ClientSocket)clientSocket);
        }

        public void receive(ClientSocket clientSocket)
        {
            byte[] rece = new byte[1024];
            while (m_isStart)
            {
                if (clientSocket.m_clientSocket != null && clientSocket.m_clientSocket.Connected)
                {
                    try
                    {
                        int recelong = clientSocket.m_clientSocket.Receive(rece, rece.Length, 0);
                        string reces = Encoding.UTF8.GetString(rece, 0, recelong);
                        // 解密
                        //reces = _3DES.DESDecrypst(reces);


                        Debug.WriteLine("客户端请求数据：" + reces);

                        this.listBox_chat.Items.Add(reces);
                    }
                    catch (SocketException ex)
                    {
                        m_clientSocketList.Remove(clientSocket);
                        this.label_peopleNum.Text = "人数:" + m_clientSocketList.Count.ToString();

                        Debug.WriteLine("与客户端连接断开:"+ clientSocket.m_id);
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

        public void sendmessage(ClientSocket clientSocket, string str)
        {
            // 加密
            //str = _3DES.DESEncrypt(str);

            try
            {
                if (clientSocket.m_clientSocket != null)
                {
                    byte[] bytes = new byte[1024];
                    bytes = Encoding.UTF8.GetBytes(str);
                    clientSocket.m_clientSocket.Send(bytes);
                }
                else
                {
                    Debug.WriteLine("错误：socketClient为空");
                }
            }
            catch (SocketException ex)
            {
                m_clientSocketList.Remove(clientSocket);
                this.label_peopleNum.Text = "人数:" + m_clientSocketList.Count.ToString();

                Debug.WriteLine("与客户端连接断开:" + clientSocket.m_id);
                Debug.WriteLine("错误日志：" + ex.Message);
            }
        }

        void destroy()
        {
            Debug.WriteLine("服务端关闭");

            m_isStart = false;

            if (m_socket != null)
            {
                m_socket.Close();
            }

            for (int i = 0; i < m_clientSocketList.Count; i++)
            {
                if (m_clientSocketList[i].m_clientSocket != null)
                {
                    m_clientSocketList[i].m_clientSocket.Close();
                }
            }
        }

        // 关闭窗口
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            destroy();
        }
    }
}

public class ClientSocket
{
    public string m_id = "";
    public Socket m_clientSocket = null;

    public ClientSocket(string id, Socket clientSocket)
    {
        m_id = id;
        m_clientSocket = clientSocket;
    }
};