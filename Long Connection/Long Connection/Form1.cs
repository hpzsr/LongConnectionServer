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
            this.textBox_send.Text = "";
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

                        ClientData clientData = new ClientData(socketback);
                        m_clientDataList.Add(clientData);
                        this.label_peopleNum.Text = "人数:" + m_clientDataList.Count.ToString();
                        
                        // 另开一个线程处理客户端的请求，防止阻塞
                        Thread t1 = new Thread(new ParameterizedThreadStart(DoTaskClient));
                        t1.Start(clientData);
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

                        string tag = "";
                        try
                        {
                            JObject jo = JObject.Parse(reces);
                            tag = jo.GetValue("tag").ToString();
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine("json格式不对");
                        }

                        // 客户端告诉服务端自己的ID
                        if (tag.CompareTo("") == 0)
                        {
                            // 是否要告诉客户端传的json不对?
                        }
                        else if (tag.CompareTo("TellSelfId") == 0)
                        {
                            onReq_TellSelfId(clientData, reces);
                        }
                        // 客户端告诉服务端自己已经初始化场景完成
                        else if (tag.CompareTo("TellSelfIsInited") == 0)
                        {
                            onReq_TellSelfIsInited(clientData);
                        }
                        // 客户端告诉服务端自己的状态信息：动作、角度
                        else if (tag.CompareTo("TellSelfStateInfo") == 0)
                        {
                            onReq_TellSelfStateInfo(clientData, reces);
                        }
                    }
                    catch (SocketException ex)
                    {
                        m_clientDataList.Remove(clientData);
                        this.label_peopleNum.Text = "人数:" + m_clientDataList.Count.ToString();

                        Debug.WriteLine("与客户端连接断开:" + clientData.m_id);
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

        //--------------------------------处理客户端请求 start-----------------------

        // 客户端告诉服务端自己的ID
        void onReq_TellSelfId(ClientData clientData, string jsonData)
        {
            // 取到客户端传来的id
            {
                JObject jo = JObject.Parse(jsonData);
                string id = jo.GetValue("uid").ToString();

                clientData.m_id = id;
            }

            // 只有进入两个人才可开始游戏
            {
                if (m_clientDataList.Count != 2)
                {
                    return;
                }
            }

            // 检测是否所有人都提交了自己的id信息
            {
                for (int i = 0; i < m_clientDataList.Count; i++)
                {
                    if (m_clientDataList[i].m_id .CompareTo("") == 0)
                    {
                        return;
                    }
                }
            }

            //--------------------------------------执行到这里说明所有人都提交了

            // 设置每个人的初始状态和位置
            {
                for (int i = 0; i < m_clientDataList.Count; i++)
                {
                    if ((i % 2) == 0)
                    {
                        m_clientDataList[i].m_data = ((int)Consts_Code.HeroAction.HeroAction_Idle).ToString() + ";90";
                    }
                    else
                    {
                        m_clientDataList[i].m_data = ((int)Consts_Code.HeroAction.HeroAction_Idle).ToString() + ";-90";
                    }
                }
            }

            // 推送自己和别人的初始状态和位置
            {
                JObject jo = new JObject();
                jo.Add("tag","InitStateInfo");

                {
                    JArray ja = new JArray();

                    for (int i = 0; i < m_clientDataList.Count; i++)
                    {
                        JObject jo2 = new JObject();
                        jo2.Add("id", m_clientDataList[i].m_id);
                        jo2.Add("data", m_clientDataList[i].m_data);
                        ja.Add(jo2);
                    }

                    jo.Add("stateInfo", ja);
                }
                // 发送消息
                for (int i = 0; i < m_clientDataList.Count; i++)
                {
                    sendmessage(m_clientDataList[i], jo.ToString());
                }
            }
        }

        // 客户端告诉服务端自己已经初始化场景完成
        void onReq_TellSelfIsInited(ClientData clientData)
        {
            clientData.m_isInited = 1;

            // 检测所有人是否都初始化场景完成
            {
                for (int i = 0; i < m_clientDataList.Count; i++)
                {
                    if (m_clientDataList[i].m_isInited == 0)
                    {
                        return;
                    }
                }
            }

            // ------------------------------------执行到这里说明所有人都初始化场景完成

            // 开始定时广播所有人的状态信息
            {
                Thread t = new Thread(SendAllPeopleStateInfo);
                t.Start();
            }
        }

        // 客户端告诉服务端自己的状态信息：动作、角度
        void onReq_TellSelfStateInfo(ClientData clientData, string jsonData)
        {
            JObject jo = JObject.Parse(jsonData);
            string data = jo.GetValue("data").ToString();

            clientData.m_data = data;
        }

        //--------------------------------处理客户端请求 end-------------------------

        void SendAllPeopleStateInfo()
        {
            m_timer = new System.Threading.Timer(callClient, "", 1000, 1000 / 30);
        }

        // 主动联系客户端
        void callClient(object data)
        {
            JObject jo = new JObject();
            jo.Add("tag", "SynAllStateInfo");

            {
                JArray ja = new JArray();

                for (int i = 0; i < m_clientDataList.Count; i++)
                {
                    JObject jo2 = new JObject();
                    jo2.Add("id", m_clientDataList[i].m_id);
                    jo2.Add("data", m_clientDataList[i].m_data);
                    ja.Add(jo2);
                }

                jo.Add("stateInfo", ja);
            }
            // 发送消息
            for (int i = 0; i < m_clientDataList.Count; i++)
            {
                sendmessage(m_clientDataList[i], jo.ToString());
            }
        }
    }
}

public class ClientData
{
    public string m_id = "";
    public string m_data = "";
    public int m_isInited = 0;
    public Socket m_clientData = null;
    
    public ClientData(Socket clientData)
    {
        m_clientData = clientData;
    }
};