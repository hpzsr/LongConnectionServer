using Newtonsoft.Json.Linq;
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
        IPAddress m_ipAddress = IPAddress.Parse("127.0.0.1");
        int m_ipPort = Convert.ToInt32("10103");
        
        List<ClientData> m_clientDataList = new List<ClientData>();

        bool m_isStart = false;
        int m_clientCount = 0;
        System.Threading.Timer m_timer;

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

            for (int i = 0; i < m_clientDataList.Count; i++)
            {
                if (m_clientDataList[i].m_clientData != null)
                {
                    m_clientDataList[i].m_clientData.Close();
                }
            }

            m_clientDataList.Clear();
            this.label_peopleNum.Text = "人数:" + m_clientDataList.Count.ToString();

            this.button_start.Enabled = true;
            this.button_stop.Enabled = false;
            this.button_fasong.Enabled = false;
        }

        // 发送消息
        private void button1_Click_1(object sender, EventArgs e)
        {
            if (m_clientDataList.Count <= 0)
            {
                Debug.WriteLine("现在没有客户端连接");

                return;
            }

            string backData = this.textBox_send.Text;
            this.listBox_chat.Items.Add("        " + backData);

            Debug.WriteLine("返回给客户端数据：" + backData);

            // 发送消息
            for (int i = 0; i < m_clientDataList.Count; i++)
            {
                sendmessage(m_clientDataList[i], backData);
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

                        ClientData clientData = new ClientData((++m_clientCount).ToString(), socketback);
                        if (m_clientDataList.Count == 1)
                        {
                            clientData.m_data = ((int)Consts_Code.HeroAction.HeroAction_Idle).ToString() + ";90";
                        }
                        else if (m_clientDataList.Count == 2)
                        {
                            clientData.m_data = ((int)Consts_Code.HeroAction.HeroAction_Idle).ToString() + ";-90";
                        }
                        m_clientDataList.Add(clientData);
                        this.label_peopleNum.Text = "人数:" + m_clientDataList.Count.ToString();

                        // 发送玩家的初始状态信息：位置、角度
                        {
                            JObject jo = new JObject();
                            jo.Add("tag", "InitGame");
                            jo.Add("data", clientData.m_data);
                            sendmessage(clientData, jo.ToString());
                        }

                        // 另开一个线程处理客户端的请求，防止阻塞
                        Thread t1 = new Thread(new ParameterizedThreadStart(DoTaskClient));
                        t1.Start(clientData);


                        // 检测人是否已经齐了
                        {
                            if (m_clientDataList.Count == 2)
                            {
                                Thread t2 = new Thread(peopleAllIn);
                                t2.Start();
                            }
                        }
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
        void DoTaskClient(object clientData)
        {
            Debug.WriteLine("获取客户端发起的请求");

            // 接收消息
            receive((ClientData)clientData);
        }

        void peopleAllIn()
        {
            m_timer = new System.Threading.Timer(callClient, "", 1000, 1000 / 30);
        }

        // 主动联系客户端
        void callClient(object data)
        {
            JArray ja = new JArray();

            for (int i = 0; i < m_clientDataList.Count; i++)
            {
                JObject jo = new JObject();
                jo.Add("id", m_clientDataList[i].m_id);
                jo.Add("data", m_clientDataList[i].m_data);
                ja.Add(jo);
            }

            // 发送消息
            for (int i = 0; i < m_clientDataList.Count; i++)
            {
                sendmessage(m_clientDataList[i], ja.ToString());
            }
        }

        public void receive(ClientData clientData)
        {
            byte[] rece = new byte[1024];
            while (m_isStart)
            {
                if (clientData.m_clientData != null && clientData.m_clientData.Connected)
                {
                    try
                    {
                        int recelong = clientData.m_clientData.Receive(rece, rece.Length, 0);
                        string reces = Encoding.UTF8.GetString(rece, 0, recelong);
                        // 解密
                        //reces = _3DES.DESDecrypst(reces);

                        Debug.WriteLine("客户端请求数据：" + reces);

                        this.listBox_chat.Items.Add(reces);
                    }
                    catch (SocketException ex)
                    {
                        m_clientDataList.Remove(clientData);
                        this.label_peopleNum.Text = "人数:" + m_clientDataList.Count.ToString();

                        Debug.WriteLine("与客户端连接断开:"+ clientData.m_id);
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

        public void sendmessage(ClientData clientData, string str)
        {
            // 加密
            //str = _3DES.DESEncrypt(str);

            try
            {
                if (clientData.m_clientData != null)
                {
                    byte[] bytes = new byte[1024];
                    bytes = Encoding.UTF8.GetBytes(str);
                    clientData.m_clientData.Send(bytes);
                }
                else
                {
                    Debug.WriteLine("错误：socketClient为空");
                }
            }
            catch (SocketException ex)
            {
                m_clientDataList.Remove(clientData);
                this.label_peopleNum.Text = "人数:" + m_clientDataList.Count.ToString();

                Debug.WriteLine("与客户端连接断开:" + clientData.m_id);
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

            for (int i = 0; i < m_clientDataList.Count; i++)
            {
                if (m_clientDataList[i].m_clientData != null)
                {
                    m_clientDataList[i].m_clientData.Close();
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

public class ClientData
{
    public string m_id = "";
    public string m_data = "";
    public Socket m_clientData = null;
    
    public ClientData(string id, Socket clientData)
    {
        m_id = id;
        m_clientData = clientData;
    }
};