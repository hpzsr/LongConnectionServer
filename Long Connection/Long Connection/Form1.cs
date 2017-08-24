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
        IPAddress m_ipAddress = IPAddress.Parse("10.224.5.110");
        int m_ipPort = Convert.ToInt32("10103");

        List<List<ClientData>> m_RoomList= new List<List<ClientData>>();

        bool m_isStart = false;
        
        //System.Threading.Timer m_timer;

        int m_frameRate = 60;

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
            this.listBox_chat.Items.Clear();

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
            this.listBox_chat.Items.Add("服务端关闭");

            m_isStart = false;

            if (m_socket != null)
            {
                m_socket.Close();
            }

            for (int a = 0; a < m_RoomList.Count; a++)
            {
                for (int i = 0; i < m_RoomList[a].Count; i++)
                {
                    if (m_RoomList[a][i].m_clientData != null)
                    {
                        m_RoomList[a][i].m_clientData.Close();
                    }
                }

                m_RoomList[a].Clear();
            }

            m_RoomList.Clear();

            this.label_peopleNum.Text = "人数:0";

            this.button_start.Enabled = true;
            this.button_stop.Enabled = false;
            this.button_fasong.Enabled = false;
        }

        // 发送消息
        private void button1_Click_1(object sender, EventArgs e)
        {
            if (m_RoomList.Count <= 0)
            {
                Debug.WriteLine("现在没有客户端连接");

                return;
            }

            string backData = this.textBox_send.Text;
            this.textBox_send.Text = "";
            this.listBox_chat.Items.Add("        " + backData);

            Debug.WriteLine("返回给客户端数据：" + backData);

            // 发送消息
            for (int a = 0; a < m_RoomList.Count; a++)
            {
                for (int i = 0; i < m_RoomList[a].Count; i++)
                {
                    sendmessage(m_RoomList[a][i], backData);
                }
            }
        }

        // 监听客户端请求
        void ListenClient()
        {
            try
            {
                IPEndPoint iPEndPoint = new IPEndPoint(m_ipAddress, m_ipPort);
                m_socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                m_socket.Bind(iPEndPoint);

                Debug.WriteLine("启动成功，开始监听");
                this.listBox_chat.Items.Add("服务端启动成功,开始监听");

                m_socket.Listen(400);

                while (m_isStart)
                {
                    try
                    {
                        Socket socketback = m_socket.Accept();

                        ClientData clientData = new ClientData(socketback);

                        {
                            bool isAddRoom = false;

                            // 现在已有房间列表里检测是否可加入
                            for (int i = 0; i < m_RoomList.Count; i++)
                            {
                                if ((m_RoomList[i].Count <= 1))
                                {
                                    bool isOK = true;
                                    for (int j = 0; j < m_RoomList[i].Count; j++)
                                    {
                                        if (m_RoomList[i][j].m_isInited == 1)
                                        {
                                            isOK = false;
                                            break;
                                        }
                                    }

                                    if (isOK)
                                    {
                                        m_RoomList[i].Add(clientData);
                                        clientData.m_clientDataList = m_RoomList[i];
                                        this.listBox_chat.Items.Add("加入现有房间");
                                        isAddRoom = true;

                                        break;
                                    }
                                }
                            }

                            // 如果已有房间列表里不可加入，则新加一个房间
                            if (!isAddRoom)
                            {
                                List<ClientData> clientDataList = new List<ClientData>();
                                m_RoomList.Add(clientDataList);

                                clientDataList.Add(clientData);
                                clientData.m_clientDataList = clientDataList;
                                this.listBox_chat.Items.Add("加入新建房间");
                            }
                        }

                        setUiPeopleNum();
                        
                        // 另开一个线程处理客户端的请求，防止阻塞
                        Thread t1 = new Thread(new ParameterizedThreadStart(DoTaskClient));
                        t1.Start(clientData);
                    }
                    catch (SocketException ex)
                    {
                        Debug.WriteLine("服务端Socket关闭");
                        Debug.WriteLine("错误日志：" + ex.Message);
                        this.listBox_chat.Items.Add("服务端Socket关闭");
                    }
                }
            }
            catch (SocketException ex)
            {
                Debug.WriteLine("服务端启动失败");
                Debug.WriteLine("错误日志：" + ex.Message);
                this.listBox_chat.Items.Add("服务端启动失败");
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
            //while (m_isStart)
            while (clientData.m_clientData != null && clientData.m_clientData.Connected)
            {
                //if (clientData.m_clientData != null && clientData.m_clientData.Connected)
                {
                    try
                    {
                        int recelong = clientData.m_clientData.Receive(rece, rece.Length, 0);
                        //if (recelong != 0)
                        {
                            string reces = Encoding.UTF8.GetString(rece, 0, recelong);
                            // 解密
                            //reces = _3DES.DESDecrypst(reces);

                            Debug.WriteLine("客户端请求数据：" + reces);

                            //this.listBox_chat.Items.Add(reces);

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

                            // json不对
                            if (tag.CompareTo("") == 0)
                            {
                                // 是否要告诉客户端传的json不对?
                            }
                            else if (tag.CompareTo("TellSelfId") == 0)
                            {
                                onReq_TellSelfId(clientData, reces);

                                this.listBox_chat.Items.Add(reces);
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
                    }
                    catch (SocketException ex)
                    {
                        List<ClientData> list = clientData.m_clientDataList;
                        list.Remove(clientData);
                        if (list.Count == 0)
                        {
                            Debug.WriteLine("房间人数为0，释放该房间");
                            this.listBox_chat.Items.Add("房间人数为0，释放该房间");
                            m_RoomList.Remove(list);
                        }

                        setUiPeopleNum();
                        
                        Debug.WriteLine("与客户端连接断开:" + clientData.m_id);
                        Debug.WriteLine("错误日志：" + ex.Message);
                        this.listBox_chat.Items.Add("与客户端连接断开:" + clientData.m_id);
                        return;
                    }
                }
            }

            Debug.WriteLine("与客户端连接断开，接收结束:" + clientData.m_id);
            this.listBox_chat.Items.Add("与客户端连接断开，接收结束:" + clientData.m_id);
            return;
        }

        public void sendmessage(ClientData clientData, string str)
        {
            // 加密
            //str = _3DES.DESEncrypt(str);
            Debug.WriteLine("发送给客户端："+ str);
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
                List<ClientData> list = clientData.m_clientDataList;
                list.Remove(clientData);
                if (list.Count == 0)
                {
                    Debug.WriteLine("房间人数为0，释放该房间");
                    this.listBox_chat.Items.Add("房间人数为0，释放该房间");
                    m_RoomList.Remove(list);
                }

                Debug.WriteLine("与客户端连接断开:" + clientData.m_id);
                Debug.WriteLine("错误日志：" + ex.Message);
                this.listBox_chat.Items.Add("与客户端连接断开:" + clientData.m_id);
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

            for (int a = 0; a < m_RoomList.Count; a++)
            {
                for (int i = 0; i < m_RoomList[a].Count; i++)
                {
                    if (m_RoomList[a][i].m_clientData != null)
                    {
                        m_RoomList[a][i].m_clientData.Close();
                    }
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
            Debug.WriteLine("客户端传过来自己的UID");

            // 取到客户端传来的id
            {
                JObject jo = JObject.Parse(jsonData);
                string id = jo.GetValue("uid").ToString();

                clientData.m_id = id;
            }

            // 只有进入两个人才可开始游戏
            {
                if (clientData.m_clientDataList.Count != 2)
                {
                    return;
                }
            }

            // 检测是否所有人都提交了自己的id信息
            {
                for (int i = 0; i < clientData.m_clientDataList.Count; i++)
                {
                    if (clientData.m_clientDataList[i].m_id .CompareTo("") == 0)
                    {
                        return;
                    }
                }
            }

            //--------------------------------------执行到这里说明所有人都提交了

            // 设置每个人的初始状态和位置
            {
                int isLeft;
                Consts_Code.HeroAction heroAction;
                float rotate;

                for (int i = 0; i < clientData.m_clientDataList.Count; i++)
                {
                    if ((i % 2) == 0)
                    {
                        isLeft = 1;
                        heroAction = Consts_Code.HeroAction.HeroAction_Idle;
                        rotate = 90;
                    }
                    else
                    {
                        isLeft = 0;
                        heroAction = Consts_Code.HeroAction.HeroAction_Idle;
                        rotate = -90;
                    }

                    clientData.m_clientDataList[i].m_data = isLeft.ToString() + ";" + ((int)heroAction).ToString() + ";" + rotate.ToString();
                }
            }

            // 推送自己和别人的初始状态和位置
            {
                JObject jo = new JObject();
                jo.Add("tag","InitStateInfo");

                {
                    JArray ja = new JArray();

                    for (int i = 0; i < clientData.m_clientDataList.Count; i++)
                    {
                        JObject jo2 = new JObject();
                        jo2.Add("id", clientData.m_clientDataList[i].m_id);
                        jo2.Add("data", clientData.m_clientDataList[i].m_data);
                        ja.Add(jo2);
                    }

                    jo.Add("stateInfo", ja);
                }
                // 发送消息
                for (int i = 0; i < clientData.m_clientDataList.Count; i++)
                {
                    sendmessage(clientData.m_clientDataList[i], jo.ToString());
                }
            }
        }

        // 客户端告诉服务端自己已经初始化场景完成
        void onReq_TellSelfIsInited(ClientData clientData)
        {
            clientData.m_isInited = 1;

            // 检测所有人是否都初始化场景完成
            {
                for (int i = 0; i < clientData.m_clientDataList.Count; i++)
                {
                    if (clientData.m_clientDataList[i].m_isInited == 0)
                    {
                        return;
                    }
                }
            }

            // ------------------------------------执行到这里说明所有人都初始化场景完成

            // 开始定时广播所有人的状态信息
            {
                //Thread t = new Thread(SendAllPeopleStateInfo);
                Thread t = new Thread(callClient);
                
                t.Start(clientData.m_clientDataList);
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

        void SendAllPeopleStateInfo(object room)
        {
            //m_timer = new System.Threading.Timer(callClient, room, 1000, 1000 / m_frameRate);
        }

        // 主动联系客户端
        void callClient(object room)
        {
            bool isNormal = true;
            while (isNormal)
            {
                Thread.Sleep(1000 / m_frameRate);
                List<ClientData> list = (List<ClientData>)room;
                // 检测房间是否需要同步
                bool isNeed = true;
                for (int i = 0; i < list.Count; i++)
                {
                    // 不需要
                    if ((list.Count != 2) || (list[i].m_isInited == 0))
                    {
                        isNeed = false;
                        break;
                    }
                }

                // 需要
                if (isNeed)
                {
                    JObject jo = new JObject();
                    jo.Add("tag", "SynAllStateInfo");

                    JArray ja = new JArray();
                    for (int i = 0; i < list.Count; i++)
                    {
                        JObject jo2 = new JObject();
                        jo2.Add("id", list[i].m_id);
                        jo2.Add("data", list[i].m_data);
                        ja.Add(jo2);
                    }

                    jo.Add("stateInfo", ja);

                    // 发送消息
                    for (int i = 0; i < list.Count; i++)
                    {
                        sendmessage(list[i], jo.ToString());
                    }
                }
                else
                {
                    isNormal = false;
                    Debug.WriteLine("不需要同步");
                }
            }

            Debug.WriteLine("有人掉线，该房间同步结束");
            this.listBox_chat.Items.Add("有人掉线，该房间同步结束");
        }

        void setUiPeopleNum()
        {
            int roomNum = 0;
            int peopleNum = 0;
            for (int a = 0; a < m_RoomList.Count; a++)
            {
                ++roomNum;

                for (int i = 0; i < m_RoomList[a].Count; i++)
                {
                    ++peopleNum;
                }
            }
            ;
            this.label_roomNum.Text = "房间:" + roomNum.ToString();
            this.label_peopleNum.Text = "人数:" + peopleNum.ToString();
        }
    }
}

public class ClientData
{
    public string m_id = "";
    public string m_data = "";
    public int m_isInited = 0;
    public Socket m_clientData = null;
    public List<ClientData> m_clientDataList = null;
    
    public ClientData(Socket clientData)
    {
        m_clientData = clientData;
    }
};