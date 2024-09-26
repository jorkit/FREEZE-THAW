using System;
using System.IO;
using System.Text;

namespace FreezeThaw.Utils
{
    public class LogTool
    {
        /// <summary>
        /// ��ӡ���ô�������Ϣ��debug log��
        /// </summary>
        /// <param name="log"></param>
        public static void DebugLogDump(string log)
        {
            string log_package = String.Format("{0}:{1}:{2}: [{3}] {4}", GetCodeContext<string>("__FILE__"), GetCodeContext<string>("__FUNC__"), GetCodeContext<int>("__LINE__"), BigBro.SceneFSM?.GetMultiplayerAuthority(), log);
            Console.WriteLine(log_package);

            /* ���汾��log */
            DateTime currentTime = DateTime.Now;
            string timeToSeconds = currentTime.ToString("yyyy-MM-dd HH:mm:ss");
            string local_log = "\r\n<" + timeToSeconds + ">\r\n" + log_package;
            LocalLogHandle("debug_log", local_log);
        }

        /// <summary>
        /// ���ڻ�ȡ��ǰ������Ϣ��
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="info"></param>
        /// <returns>����ָ���ĵ�ǰ������Ϣ��</returns>
        private static T GetCodeContext<T>(string info)
        {
            try
            {
                System.Diagnostics.StackTrace st = new System.Diagnostics.StackTrace(2, true);

                switch (info)
                {
                    case "__FILE__":
                        return (T)(object)st.GetFrame(0).GetFileName();
                    case "__FUNC__":
                        return (T)(object)st.GetFrame(0).GetMethod().Name;
                    case "__LINE__":
                        return (T)(object)st.GetFrame(0).GetFileLineNumber();
                }
                Console.WriteLine("GetCodeContext��������");
            }
            catch (Exception)
            {
                Console.WriteLine("��ȡ�����������쳣��");
            }

            return default;
        }

        /// <summary>
        /// ��ASCIIֵת��Ϊ��Ӧ���ַ����ϳ��ַ���
        /// </summary>
        /// <param name="ascii"></param>
        /// <returns>�ɹ�����ת������ַ�����ʧ�ܷ���NULL</returns>
        public static string AsciiToString(string ascii)
        {
            if (ascii == null)
                return null;

            try
            {
                string[] ascii_arr = ascii.Split(' ');
                StringBuilder hexString = new StringBuilder();

                foreach (string c in ascii_arr)
                {
                    var c_tmp = Convert.ToInt16(c, 16);
                    hexString.Append((char)c_tmp);
                }
                return hexString.ToString();
            }
            catch (Exception)
            {
                LogTool.DebugLogDump("AsciiToString faild, ASCII: " + ascii);
                return null;
            }
        }

        /// <summary>
        /// ����log����
        /// </summary>
        /// <param name="log_file_name"></param>
        /// <param name="log"></param>
        public static void LocalLogHandle(string log_file_name, string log)
        {
            if (log_file_name == null && string.IsNullOrWhiteSpace(log))
            {
                return;
            }
            
            try
            {
                FileStream fs;
                int i = 0;
                do
                {
                    string path = "C:\\" + log_file_name + i + ".txt";
                    /* ���汾��log */
                    fs = new FileStream(path, FileMode.Append, FileAccess.Write, FileShare.None);
                    if (fs == null)
                    {
                        Console.WriteLine("{0}:{1}:{2}: {3}", GetCodeContext<string>("__FILE__"), GetCodeContext<string>("__FUNC__"), GetCodeContext<int>("__LINE__"), "FileStream��ʧ�ܣ�");
                        return;
                    }
                    /* log��С������30M���� */
                    if (fs.Length > 1024 * 1024 * 30)
                    {
                        Console.WriteLine("{0}:{1}:{2}: {3}", GetCodeContext<string>("__FILE__"), GetCodeContext<string>("__FUNC__"), GetCodeContext<int>("__LINE__"), "Log�ļ���С����30M���������ļ���");
                        fs.Close();
                        i++;
                    }
                    else
                    {
                        break;
                    }
                } while (true);

                StreamWriter sw = new StreamWriter(fs);
                if (sw == null)
                {
                    Console.WriteLine("{0}:{1}:{2}: {3}", GetCodeContext<string>("__FILE__"), GetCodeContext<string>("__FUNC__"), GetCodeContext<int>("__LINE__"), "StreamWriter��ʧ�ܣ�");
                    return;
                }
                sw.Write(log);
                sw.Close();
                fs.Close();
            }
            catch (Exception)
            {
                Console.WriteLine("{0}:{1}:{2}: {3}", GetCodeContext<string>("__FILE__"), GetCodeContext<string>("__FUNC__"), GetCodeContext<int>("__LINE__"), "����log�����쳣��");
                return;
            }
        }
    }
}