using System;
using System.Collections.Generic;
using System.Text;
using ZGuard;
using ZPort;

namespace TestDevices
{

    public class TestDevice
    {

        public static EventHandler ReportHandler;

        public static void OnReportHandler(string strMessage)
        {
            if (ReportHandler != null)
                ReportHandler(strMessage, new EventArgs());
        }

        //public const ZP_PORT_TYPE CvtPortType = ZP_PORT_TYPE.ZP_PORT_COM;
        //public const string CvtPortName = "COM3";
        //public const Byte CtrAddr = 3;
        public const ZP_PORT_TYPE CvtPortType = ZP_PORT_TYPE.ZP_PORT_IP;
        public const string CvtPortName = @"192.168.112.253:1000";
        public const Byte CtrAddr = 2;

        public static IntPtr m_hCtr;
        public static int m_nCtrMaxEvents;
        public static bool m_fProximity;
        public static UInt32 m_nCtrFlags;
        public static bool m_fCtrNotifyEnabled;
        public static int m_nAppReadEventIdx;

        //int hr;
        //IntPtr hCvt;
        //string msg;
        public static bool TestDeviceAccess(out IntPtr hCvt, ref ZG_CTR_INFO rCtrInfo)
        {
            hCvt = new IntPtr(0);
            m_hCtr = new IntPtr(0);
            int hr;
            var msg = "";
            //ZG_CTR_INFO rCtrInfo = new ZG_CTR_INFO();
            //var paramFive = rCtrInfo.nMaxEvents;

            // Проверяем версию SDK
            UInt32 nVersion = ZGIntf.ZG_GetVersion();
            if ((((nVersion & 0xFF)) != ZGIntf.ZG_SDK_VER_MAJOR) || (((nVersion >> 8) & 0xFF) != ZGIntf.ZG_SDK_VER_MINOR))
            {
                Console.WriteLine("Неправильная версия SDK Guard.");
                Console.ReadLine();
                return true;
            }

            hr = ZGIntf.ZG_Initialize(ZPIntf.ZP_IF_NO_MSG_LOOP);
            if (hr < 0)
            {
                //Console.WriteLine("Ошибка ZG_Initialize ({0}).", hr);
                msg = string.Format("Ошибка ZG_Initialize ({0}).", hr);
                Console.WriteLine(msg);
                OnReportHandler(msg); // и т.д. по желанию
                Console.ReadLine();
                return true;
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
                    //Console.WriteLine("Ошибка ZG_Cvt_Open ({0}).", hr);
                    msg = string.Format("Ошибка ZG_Cvt_Open ({0}).", hr);
                    Console.WriteLine(msg);
                    OnReportHandler(msg); // и т.д. по желанию
                    Console.ReadLine();
                    return true;
                }
                //ZG_CTR_INFO rCtrInfo = new ZG_CTR_INFO();
                hr = ZGIntf.ZG_Ctr_Open(ref m_hCtr, hCvt, CtrAddr, 0, ref rCtrInfo);
                if (hr < 0)
                {
                    //Console.WriteLine("Ошибка ZG_Ctr_Open ({0}).", hr);
                    msg = string.Format("Ошибка ZG_Ctr_Open ({0}).", hr);
                    Console.WriteLine(msg);
                    OnReportHandler(msg); // и т.д. по желанию
                    Console.ReadLine();
                    return true;
                }
            }
            catch (Exception ex)
            {
                OnReportHandler(ex.Message);
                throw; // бросаемся дальше
            }
            return false;
        }
    }
}
