using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using ZGuard;
using ZPort;

using System.Runtime.InteropServices;
using System.Globalization;
using System.Threading;


namespace VizorNEW
{
    public partial class Form1 : Form
    {

        public static readonly string[] CtrTypeStrs = { 
                                                          "", 
                                                          "Gate 2000", 
                                                          "Matrix II Net", 
                                                          "Matrix III Net", 
                                                          "Z5R Net", 
                                                          "Z5R Net 8000", 
                                                          "Guard Net", 
                                                          "Z-9 EHT Net", 
                                                          "EuroLock EHT net", 
                                                          "Z5R Web", 
                                                          "Matrix II Wi-Fi"
                                                      };

        //public const ZP_PORT_TYPE CvtPortType = ZP_PORT_TYPE.ZP_PORT_COM;
        //public const string CvtPortName = "COM3";
        //public const Byte CtrAddr = 3;
        public const ZP_PORT_TYPE CvtPortType = ZP_PORT_TYPE.ZP_PORT_IP;
        public const string CvtPortName = @"192.168.112.253:1000";
        public const Byte CtrAddr = 2;

        public static IntPtr m_hCtr;
        public static int m_nCtrMaxBanks;


        

        public static readonly string[] KeyModeStrs = { "Touch Memory", "Proximity" };
        public static readonly string[] KeyTypeStrs = { "", "Обычный", "Блокирующий", "Мастер" };

        //public static IntPtr m_hCtr = IntPtr.Zero;
        //public static int m_nCtrMaxBanks;
        public static bool m_fProximity;
        public static int m_nFoundKeyIdx;
        public static Byte[] m_rFindNum;





        public Form1()
        {
            InitializeComponent();
        }

        static void DoOpenLock(int nLockN)
        {
            int hr;
            hr = ZGIntf.ZG_Ctr_OpenLock(m_hCtr, nLockN);
            if (hr < 0)
            {
                Console.WriteLine("Ошибка ZG_Ctr_OpenLock ({0}).", hr);
                Console.ReadLine();
                return;
            }
            Console.WriteLine("Успешно.");
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // Проверяем версию SDK
            UInt32 nVersion = ZGIntf.ZG_GetVersion();
            if ((((nVersion & 0xFF)) != ZGIntf.ZG_SDK_VER_MAJOR) || (((nVersion >> 8) & 0xFF) != ZGIntf.ZG_SDK_VER_MINOR))
            {
                Console.WriteLine("Неправильная версия SDK Guard.");
                Console.ReadLine();
                return;
            }

            IntPtr hCvt = new IntPtr(0);
            m_hCtr = new IntPtr(0);
            int hr;

            hr = ZGIntf.ZG_Initialize(ZPIntf.ZP_IF_NO_MSG_LOOP);
            if (hr < 0)
            {
                Console.WriteLine("Ошибка ZG_Initialize ({0}).", hr);
                Console.ReadLine();
                return;
            }
            try
            {
                ZG_CVT_INFO rInfo = new ZG_CVT_INFO();
                ZG_CVT_OPEN_PARAMS rOp = new ZG_CVT_OPEN_PARAMS();
                rOp.nPortType = CvtPortType;
                rOp.pszName = CvtPortName;
                rOp.nSpeed = ZG_CVT_SPEED.ZG_SPEED_57600;
                hr = ZGIntf.ZG_Cvt_Open(ref hCvt, ref rOp, rInfo);
                if (hr < 0)
                {
                    Console.WriteLine("Ошибка ZG_Cvt_Open ({0}).", hr);
                    Console.ReadLine();
                    return;
                }
                ZG_CTR_INFO rCtrInfo = new ZG_CTR_INFO();
                hr = ZGIntf.ZG_Ctr_Open(ref m_hCtr, hCvt, CtrAddr, 0, ref rCtrInfo);
                if (hr < 0)
                {
                    Console.WriteLine("Ошибка ZG_Ctr_Open ({0}).", hr);
                    Console.ReadLine();
                    return;
                }
                m_nCtrMaxBanks = ((rCtrInfo.nFlags & ZGIntf.ZG_CTR_F_2BANKS) != 0) ? 2 : 1;
                m_fProximity = ((rCtrInfo.nFlags & ZGIntf.ZG_CTR_F_PROXIMITY) != 0);
                Console.WriteLine("{0} адрес: {1}, с/н: {2}, v{3}.{4}, Количество банков: {5}, Тип ключей: {6}.",
                    CtrTypeStrs[(int)rCtrInfo.nType],
                    rCtrInfo.nAddr,
                    rCtrInfo.nSn,
                    rCtrInfo.nVersion & 0xff, (rCtrInfo.nVersion >> 8) & 0xff,
                    m_nCtrMaxBanks,
                    KeyModeStrs[m_fProximity ? 1 : 0]);
                m_oEvent = new ManualResetEvent(false);
                ZG_CTR_NOTIFY_SETTINGS rNS = new ZG_CTR_NOTIFY_SETTINGS(
                    ZGIntf.ZG_NF_CTR_KEY_TOP, m_oEvent.SafeWaitHandle, IntPtr.Zero, 0,
                    0,
                    3000, // Период проверки верхней границы ключей
                    0);
                hr = ZGIntf.ZG_Ctr_SetNotification(m_hCtr, rNS);
                if (hr < 0)
                {
                    Console.WriteLine("Ошибка ZG_Ctr_SetNotification ({0}).", hr);
                    Console.ReadLine();
                    return;
                }
                StartNotifyThread();
                Console.WriteLine("-----");
                //while (true)
                //{
                    //DoOpenLock(0);
                    /*
                    Console.WriteLine("Введите номер команды:");
                    Console.WriteLine("1 - показать времена замков");
                    Console.WriteLine("2 - установить времена замков...");
                    Console.WriteLine("3 - открыть замок (Вход)");
                    Console.WriteLine("4 - открыть замок (Выход)");
                    Console.WriteLine("9 - восстановить заводские настройки (для всех банков)");
                    Console.WriteLine("0 - выход");
                    s = Console.ReadLine();
                    if (s != "")
                    {
                        Console.WriteLine();
                        switch (Convert.ToInt32(s))
                        {
                            case 1:
                                //ShowLockTimes();
                                break;
                            case 2:
                                //DoSetLockTimes();
                                break;
                            case 3:
                                DoOpenLock(0);
                                break;
                            case 4:
                                DoOpenLock(1);
                                break;
                            case 9:
                                //DoRestoreFactorySettings();
                                break;
                            case 0:
                                return;
                            default:
                                Console.WriteLine("Неверная команда.");
                                break;
                        }
                    }
                    Console.WriteLine("-----");
                    */
                //}
            }
            finally
            {
                if (m_hCtr != IntPtr.Zero)
                    ZGIntf.ZG_CloseHandle(m_hCtr);
                if (hCvt != IntPtr.Zero)
                    ZGIntf.ZG_CloseHandle(hCvt);
                ZGIntf.ZG_Finalyze();
            }
        }

        static int CheckNotifyMsgs()
        {
            int hr;
            UInt32 nMsg = 0;
            IntPtr nMsgParam = IntPtr.Zero;
            while ((hr = ZGIntf.ZG_Ctr_GetNextMessage(m_hCtr, ref nMsg, ref nMsgParam)) == ZGIntf.S_OK)
            {
                switch (nMsg)
                {
                    case ZGIntf.ZG_N_CTR_KEY_TOP:
                        {
                            ZG_N_KEY_TOP_INFO pInfo = (ZG_N_KEY_TOP_INFO)Marshal.PtrToStructure(nMsgParam, typeof(ZG_N_KEY_TOP_INFO));
                            Console.WriteLine("==> Банк {0}: верхняя граница ключей изменена ({1} -> {2}).",
                                pInfo.nBankN, pInfo.nOldTopIdx, pInfo.nNewTopIdx);
                        }
                        break;
                }
            }
            if (hr == ZPIntf.ZP_S_NOTFOUND)
                hr = ZGIntf.S_OK;
            return hr;
        }

        static ManualResetEvent m_oEvent = null;
        static bool m_fThreadActive;
        static Thread m_oThread = null;
        static void DoNotifyWork()
        {
            while (m_fThreadActive)
            {
                if (m_oEvent.WaitOne())
                {
                    m_oEvent.Reset();
                    if (m_hCtr != IntPtr.Zero)
                        CheckNotifyMsgs();
                }
            }
        }

        static void StartNotifyThread()
        {
            if (m_oThread != null)
                return;
            m_fThreadActive = true;
            m_oThread = new Thread(DoNotifyWork);
            m_oThread.Start();
        }
        static void StopNotifyThread()
        {
            if (m_oThread == null)
                return;
            m_fThreadActive = false;
            m_oEvent.Set();
            // Wait until oThread finishes. Join also has overloads
            // that take a millisecond interval or a TimeSpan object.
            m_oThread.Join();
            m_oThread = null;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            int hr;
            int i, j, nCount;
            int nTop = 0;
            ZG_CTR_KEY[] aKeys = new ZG_CTR_KEY[6];
            ZG_CTR_KEY pKey;

            for (i = 0; i < m_nCtrMaxBanks; i++)
            {
                //Console.WriteLine("------------");
                //Console.WriteLine("Банк № {0}:", i);
                
                // Не работает
                hr = ZGIntf.ZG_Ctr_GetKeyTopIndex(m_hCtr, ref nTop, i);
                if (hr < 0)
                {
                    //Console.WriteLine("Ошибка ZG_Ctr_GetKeyTopIndex (банк № {0}) ({1}).", i, hr);
                    //Console.ReadLine();
                    return;
                }
                if (nTop == 0)
                {
                    //Console.WriteLine("список пуст.");
                    continue;
                }
                for (j = 0; j < nTop; j++)
                {
                    if ((j % aKeys.Length) == 0)
                    {
                        nCount = (nTop - j);
                        if (nCount > aKeys.Length)
                            nCount = aKeys.Length;
                        hr = ZGIntf.ZG_Ctr_ReadKeys(m_hCtr, j, aKeys, nCount, null, IntPtr.Zero, i);
                        if (hr < 0)
                        {
                            //Console.WriteLine("Ошибка ZG_Ctr_ReadKeys (банк № {0}) ({1}).", i, hr);
                            //Console.ReadLine();
                            return;
                        }
                    }
                    pKey = aKeys[j % aKeys.Length];
                    /*if (pKey.fErased)
                        Console.WriteLine("{0} стерт.", j);
                    else
                    {
                        Console.WriteLine("{0} {1}, {2}, доступ: {3:X2}h.",
                            j,
                            ZGIntf.CardNumToStr(pKey.rNum, m_fProximity),
                            KeyTypeStrs[(int)pKey.nType],
                            pKey.nAccess);
                    }*/
                }
            }
            //Console.WriteLine("Успешно.");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //CtrKeys.Program.Main(new []{"1", "0"});

        }
    }
}
