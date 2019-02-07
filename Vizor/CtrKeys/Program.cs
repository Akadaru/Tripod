﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Globalization;
using System.Threading;
using ZGuard;
using ZPort;

namespace CtrKeys
{
    public class Program
    {

        public static EventHandler ReportHandler;

        public static void OnReportHandler(string strMessage)
        {
            if (ReportHandler != null)
                ReportHandler(strMessage, new EventArgs());
        }

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
        public static readonly string[] KeyModeStrs = { "Touch Memory", "Proximity" };
        public static readonly string[] KeyTypeStrs = { "", "Обычный", "Блокирующий", "Мастер" };


        //public const ZP_PORT_TYPE CvtPortType = ZP_PORT_TYPE.ZP_PORT_COM;
        //public const string CvtPortName = "COM11";
        //public const Byte CtrAddr = 2;
        public const ZP_PORT_TYPE CvtPortType = ZP_PORT_TYPE.ZP_PORT_IP;
        public const string CvtPortName = @"192.168.112.253:1000";
        public const Byte CtrAddr = 2;

        public static IntPtr m_hCtr = IntPtr.Zero;
        public static int m_nCtrMaxBanks;
        public static bool m_fProximity;
        public static int m_nFoundKeyIdx;
        public static Byte[] m_rFindNum;

        internal static void ShowKeys()
        {
            int hr;
            int i, j, nCount;
            int nTop = 0;
            ZG_CTR_KEY[] aKeys = new ZG_CTR_KEY[6];
            ZG_CTR_KEY pKey;

            var msg = "";

            for (i = 0; i < m_nCtrMaxBanks; i++)
            {
                //var msg = "";
                msg = string.Format("------------");
                Console.WriteLine(msg);
                OnReportHandler(msg); // и т.д. по желанию
                msg = string.Format("Банк № {0}:", i);
                Console.WriteLine(msg);
                OnReportHandler(msg); // и т.д. по желанию

                hr = ZGIntf.ZG_Ctr_GetKeyTopIndex(m_hCtr, ref nTop, i);
                if (hr < 0)
                {
                    //Console.WriteLine("Ошибка ZG_Ctr_GetKeyTopIndex (банк № {0}) ({1}).", i, hr);
                    msg = string.Format("Ошибка ZG_Ctr_GetKeyTopIndex (банк № {0}) ({1}).", i, hr);
                    Console.WriteLine(msg);
                    OnReportHandler(msg); // и т.д. по желанию
                    continue;
                    Console.ReadLine();
                    return;
                }
                if (nTop == 0)
                {
                    //Console.WriteLine("список пуст.");
                    msg = string.Format("список пуст.");
                    Console.WriteLine(msg);
                    OnReportHandler(msg); // и т.д. по желанию
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
                            msg = string.Format("Ошибка ZG_Ctr_ReadKeys (банк № {0}) ({1}).", i, hr);
                            Console.WriteLine(msg);
                            OnReportHandler(msg); // и т.д. по желанию
                            Console.ReadLine();
                            return;
                        }
                    }
                    pKey = aKeys[j % aKeys.Length];
                    if (pKey.fErased)
                        Console.WriteLine("{0} стерт.", j);
                    else
                    {
                        /*
                        Console.WriteLine("{0} {1}, {2}, доступ: {3:X2}h.",
                            j,
                            ZGIntf.CardNumToStr(pKey.rNum, m_fProximity),
                            KeyTypeStrs[(int)pKey.nType],
                            pKey.nAccess);
                        */
                        msg = string.Format("{0} {1}, {2}, доступ: {3:X2}h.",
                           j,
                           ZGIntf.CardNumToStr(pKey.rNum, m_fProximity),
                           KeyTypeStrs[(int)pKey.nType],
                           pKey.nAccess);
                        Console.WriteLine(msg);
                        OnReportHandler(msg);

                    }
                }
            }
            //Console.WriteLine("Успешно.");
            msg = string.Format("Успешно.");
            Console.WriteLine(msg);
            OnReportHandler(msg); // и т.д. по желанию
        }

        static bool ParseKeyNum(ref Byte[] rKeyNum, string sText)
        {
            string[] aValues = sText.Split(',');
            if (aValues.Length == 2)
            {
                Byte nGroup = Convert.ToByte(aValues[0]);
                UInt16 nNumber = Convert.ToUInt16(aValues[1]);
                rKeyNum[0] = 3;
                rKeyNum[1] = (Byte)nNumber;
                rKeyNum[2] = (Byte)(nNumber >> 8);
                rKeyNum[3] = nGroup;
            }
            else
            {
                int j = 1;
                for (int i = sText.Length - 2; i >= 0; i -= 2)
                {
                    rKeyNum[j] = byte.Parse(string.Concat(sText[i], sText[i + 1]), NumberStyles.HexNumber);
                    j++;
                    if (j > 6)
                        break;
                }
                rKeyNum[0] = (Byte)(j - 1);
            }
            return true;
        }

        static bool FindKeyEnum(int nIdx, ref ZG_CTR_KEY pKey, int nPos, int nMax, IntPtr pUserData)
        {
            bool fFound = true;
            int nCnt = (m_rFindNum[0] < 6) ? m_rFindNum[0] : 6;
            for (int i = 1; i <= nCnt; ++i)
                if (m_rFindNum[i] != pKey.rNum[i])
                {
                    fFound = false;
                    break;
                }
            if (fFound)
            {
                m_nFoundKeyIdx = nIdx;
                return false;
            }
            return true;
        }

        static void DoFindKeyByNumber()
        {
            string s;

            Console.WriteLine("Введите № банка, номер ключа (-1 последний поднесенный):");
            s = Console.ReadLine();
            string[] aValues = s.Split(',');
            if (aValues.Length < 2)
            {
                Console.WriteLine("Некорректный ввод.");
                return;
            }
            int hr;
            int nBankN;
            m_rFindNum = new Byte[16];
            nBankN = Convert.ToInt32(aValues[0]);
            if (aValues[1] == "-1")
            {
                hr = ZGIntf.ZG_Ctr_ReadLastKeyNum(m_hCtr, m_rFindNum);
                if (hr < 0)
                {
                    Console.WriteLine("Ошибка ZG_Ctr_ReadLastKeyNum ({0}).", hr);
                    Console.ReadLine();
                    return;
                }
            }
            else if (!ParseKeyNum(ref m_rFindNum, aValues[1]))
            {
                Console.WriteLine("Некорректный ввод.");
                return;
            }
            m_nFoundKeyIdx = -1;
            hr = ZGIntf.ZG_Ctr_EnumKeys(m_hCtr, 0, FindKeyEnum, IntPtr.Zero, nBankN);
            if (hr < 0)
            {
                Console.WriteLine("Ошибка ZG_Ctr_EnumKeys ({0}).", hr);
                Console.ReadLine();
                return;
            }
            if (m_nFoundKeyIdx != -1)
                Console.WriteLine("Key {0} found (index={0}).",
                    ZGIntf.CardNumToStr(m_rFindNum, m_fProximity), m_nFoundKeyIdx);
            else
                Console.WriteLine("Key {0} not found.",
                    ZGIntf.CardNumToStr(m_rFindNum, m_fProximity));
        }

        static void ShowKeyTopIndex()
        {
            int nKeyIdx;
            int hr;

            Console.WriteLine("Получение верхней границы ключей...");
            nKeyIdx = 0;
            for (int i = 0; i < m_nCtrMaxBanks; i++)
            {
                hr = ZGIntf.ZG_Ctr_GetKeyTopIndex(m_hCtr, ref nKeyIdx, i);
                if (hr < 0)
                {
                    Console.WriteLine("Ошибка ZG_Ctr_GetKeyTopIndex ({0}).", hr);
                    Console.ReadLine();
                    break; ;
                }
                Console.WriteLine("Банк {0}: {1}", i, nKeyIdx);
            }
            Console.WriteLine("Завершено.");
        }

        static void DoSetKey()
        {
            int nBankN, nKeyIdx, nKeyType, nKeyAccess;
            string s;

            //Console.WriteLine("Введите № банка, индекс ключа (-1 верхняя граница), " +
            //    "номер ключа (-1 последний поднесенный), тип (1-обычный,2-блокирующий,3-мастер), " + 
            //    "доступ (hex):");
            var msg = "";
            msg = string.Format("Введите № банка, индекс ключа (-1 верхняя граница), " +
                "номер ключа (-1 последний поднесенный), тип (1-обычный,2-блокирующий,3-мастер), " +
                "доступ (hex):");
            Console.WriteLine(msg);
            OnReportHandler(msg); // и т.д. по желанию
            s = Console.ReadLine();
            string[] aValues = s.Split(',');
            if (aValues.Length < 5)
            {
                Console.WriteLine("Некорректный ввод.");
                return;
            }
            nBankN = Convert.ToInt32(aValues[0]);
            nKeyIdx = Convert.ToInt32(aValues[1]);
            nKeyType = Convert.ToInt32(aValues[3]);
            nKeyAccess = Convert.ToInt32(aValues[4], 16);

            int hr;
            if (nKeyIdx == -1)
            {
                hr = ZGIntf.ZG_Ctr_GetKeyTopIndex(m_hCtr, ref nKeyIdx, nBankN);
                if (hr < 0)
                {
                    Console.WriteLine("Ошибка ZG_Ctr_GetKeyTopIndex ({0}).", hr);
                    Console.ReadLine();
                    return;
                }
            }
            ZG_CTR_KEY[] aKeys = new ZG_CTR_KEY[1];
            aKeys[0].nType = (ZG_CTR_KEY_TYPE)nKeyType;
            aKeys[0].nAccess = (Byte)nKeyAccess;
            aKeys[0].rNum = new Byte[16];
            //aKeys[0].aData1 = new Byte[4];
            if (aValues[2] == "-1")
            {
                hr = ZGIntf.ZG_Ctr_ReadLastKeyNum(m_hCtr, aKeys[0].rNum);
                if (hr < 0)
                {
                    Console.WriteLine("Ошибка ZG_Ctr_ReadLastKeyNum ({0}).", hr);
                    Console.ReadLine();
                    return;
                }
            }
            else if (!ParseKeyNum(ref aKeys[0].rNum, aValues[2]))
            {
                Console.WriteLine("Некорректный ввод.");
                return;
            }
            hr = ZGIntf.ZG_Ctr_WriteKeys(m_hCtr, nKeyIdx, aKeys, 1, null, IntPtr.Zero, nBankN);
            if (hr < 0)
            {
                Console.WriteLine("Ошибка ZG_Ctr_WriteKeys ({0}).", hr);
                Console.ReadLine();
                return;
            }
            Console.WriteLine("Успешно.");
        }

        static void DoClearKey()
        {
            int nBankN, nKeyIdx;
            string s;

            Console.WriteLine("Введите № банка, индекс ключа (-1 ключ в хвосте):");
            s = Console.ReadLine();
            string[] aValues = s.Split(',');
            if (aValues.Length < 2)
            {
                Console.WriteLine("Некорректный ввод.");
                return;
            }
            nBankN = Convert.ToInt32(aValues[0]);
            nKeyIdx = Convert.ToInt32(aValues[1]);
            int hr;
            if (nKeyIdx == -1)
            {
                int nTop = 0;
                hr = ZGIntf.ZG_Ctr_GetKeyTopIndex(m_hCtr, ref nTop, nBankN);
                if (hr < 0)
                {
                    Console.WriteLine("Ошибка ZG_Ctr_GetKeyTopIndex ({0}).", hr);
                    Console.ReadLine();
                    return;
                }
                if (nTop == 0)
                {
                    Console.WriteLine("Список ключей пуст.");
                    return;
                }
                nKeyIdx = (nTop - 1);
            }
            hr = ZGIntf.ZG_Ctr_ClearKeys(m_hCtr, nKeyIdx, 1, null, IntPtr.Zero, nBankN);
            if (hr < 0)
            {
                Console.WriteLine("Ошибка ZG_Ctr_ClearKeys ({0}).", hr);
                Console.ReadLine();
                return;
            }
            Console.WriteLine("Успешно.");
        }

        static void DoClearAllKeys()
        {
            int nBankN;
            string s;

            Console.WriteLine("Введите № банка:");
            s = Console.ReadLine();
            if (s == "")
            {
                Console.WriteLine("Некорректный ввод.");
                return;
            }
            nBankN = Convert.ToInt32(s);
            int nTop = 0;
            int hr;
            hr = ZGIntf.ZG_Ctr_GetKeyTopIndex(m_hCtr, ref nTop, nBankN);
            if (hr < 0)
            {
                Console.WriteLine("Ошибка ZG_Ctr_GetKeyTopIndex ({0}).", hr);
                Console.ReadLine();
                return;
            }
            if (nTop == 0)
            {
                Console.WriteLine("Список ключей пуст.");
                return;
            }
            Console.WriteLine("Очистка...");
            hr = ZGIntf.ZG_Ctr_ClearKeys(m_hCtr, 0, nTop, null, IntPtr.Zero, nBankN);
            if (hr < 0)
            {
                Console.WriteLine("Ошибка ZG_Ctr_ClearKeys ({0}).", hr);
                Console.ReadLine();
                return;
            }
            Console.WriteLine("Успешно.");
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

        internal static void Main(string[] args)
        {
            ZG_CTR_INFO rCtrInfo = new ZG_CTR_INFO();
            int hr;
            IntPtr hCvt;
            string msg;
            //if (TestDeviceAccess(out hCvt, ref rCtrInfo)) return;
            //if (TestDevice.TestDeviceAccess(out hCvt, ref rCtrInfo)) return; // Тестирование Доступа К Устройству


            try
            {
                m_nCtrMaxBanks = ((rCtrInfo.nFlags & ZGIntf.ZG_CTR_F_2BANKS) != 0) ? 2 : 1;
                m_fProximity = ((rCtrInfo.nFlags & ZGIntf.ZG_CTR_F_PROXIMITY) != 0);
                // в любом месте, где мы хотим видеть текст не только в консоли, но и в результате вывода, вызываем OnReportHandler
                msg = string.Format("{0} адрес: {1}, с/н: {2}, v{3}.{4}, Количество банков: {5}, Тип ключей: {6}.",
                   CtrTypeStrs[(int)rCtrInfo.nType],
                   rCtrInfo.nAddr,
                   rCtrInfo.nSn,
                   rCtrInfo.nVersion & 0xff, (rCtrInfo.nVersion >> 8) & 0xff,
                   m_nCtrMaxBanks,
                   KeyModeStrs[m_fProximity ? 1 : 0]);
                Console.WriteLine(msg);
                OnReportHandler(msg);
            }
            catch (Exception ex)
            {
                OnReportHandler(ex.Message);
                throw; // бросаемся дальше
            }
            

            try
            {
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
                string s;

                while (true)
                {
                    Console.WriteLine("Введите номер команды:");
                    Console.WriteLine("1 - показать ключи");
                    Console.WriteLine("2 - поиск ключа по номеру...");
                    Console.WriteLine("3 - показать верхнюю границу ключей");
                    Console.WriteLine("6 - установка ключа...");
                    Console.WriteLine("7 - стирание ключа...");
                    Console.WriteLine("8 - стирание всех ключей...");
                    Console.WriteLine("0 - выход");
                    s = Console.ReadLine();
                    bool returnAfterExecute = false;
                    if (args != null && args.Length > 0)
                    {
                        s = args[0];
                        // для примера чисто тебе напишу вариант с одним параметром, ты уже сам сможешь придумать обработку нескольких или вызов методов отдельно от этого шлака
                        returnAfterExecute = true;
                    }
                    if (s != "")
                    {
                        Console.WriteLine();
                        switch (Convert.ToInt32(s))
                        {
                            case 1:
                                ShowKeys();
                                if (returnAfterExecute)
                                    return;
                                break;
                            case 2:
                                DoFindKeyByNumber();
                                break;
                            case 3:
                                ShowKeyTopIndex();
                                break;
                            case 6:
                                DoSetKey();
                                if (returnAfterExecute)
                                    return;
                                break;
                            case 7:
                                DoClearKey();
                                break;
                            case 8:
                                DoClearAllKeys();
                                break;
                            case 0:
                                return;
                            default:
                                Console.WriteLine("Неверная команда.");
                                break;
                        }
                    }
                    Console.WriteLine("-----");
                }
            }
            catch (Exception ex)
            {
                OnReportHandler(ex.Message);
                throw; // бросаемся дальше
            }
            finally
            {
                StopNotifyThread();
                if (m_hCtr != IntPtr.Zero)
                    ZGIntf.ZG_CloseHandle(m_hCtr);
                if (hCvt != IntPtr.Zero)
                    ZGIntf.ZG_CloseHandle(hCvt);
                ZGIntf.ZG_Finalyze();
            }
        }


    }
}
