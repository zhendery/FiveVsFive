using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Net;
using System.IO;
using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace FiveVsFive
{
    class IPManager
    {
        //#region “64位”的查找表
        //static Dictionary<int, char> ASPECT_64 = new Dictionary<int, char>() { 
        //    {0,'0'},{1,'1'},{2,'2'},{3,'3'},{4,'4'},{5,'5'},{6,'6'},{7,'7'},{8,'8'},{9,'9'},

        //    {10,'A'},{11,'B'},{12,'C'},{13,'D'},{14,'E'},{15,'F'},{16,'G'},{17,'H'},{18,'I'},{19,'J'},{20,'K'},{21,'L'},{22,'M'},
        //    {23,'N'},{24,'O'},{25,'P'},{26,'Q'},{27,'R'},{28,'S'},{29,'T'},{30,'U'},{31,'V'},{32,'W'},{33,'X'},{34,'Y'},{35,'Z'},
            
        //    {36,'a'},{37,'b'},{38,'c'},{39,'d'},{40,'e'},{41,'f'},{42,'g'},{43,'h'},{44,'i'},{45,'j'},{46,'k'},{47,'l'},{48,'m'},
        //    {49,'n'},{50,'o'},{51,'p'},{52,'q'},{53,'r'},{54,'s'},{55,'t'},{56,'u'},{57,'v'},{58,'w'},{59,'x'},{60,'y'},{61,'z'},

        //    {62,','},{63,'.'},
        //};
        //#endregion

        //public static string zipIp(string ip)//压缩
        //{
        //    byte[] ips = new byte[4];
        //    string[] ipsString = ip.Split(new char[] { '.' });
        //    for (short i = 0; i < 4; ++i)
        //        ips[i] = Byte.Parse(ipsString[i]);

        //    int content = BitConverter.ToInt32(ips, 0);
        //    return getInt64(content);
        //}
        //public static string unZipIp(string ip)//解压
        //{
        //    int content = getInt64(ip);

        //    byte[] bits = BitConverter.GetBytes(content);
        //    StringBuilder newIP = new StringBuilder();
        //    for (short i = 0; i < 4; ++i)
        //        newIP.Append(bits[i]).Append('.');
        //    newIP.Remove(newIP.Length - 1, 1);

        //    return newIP.ToString();
        //}
        //public static string getWlanIP()//获得广域网IP
        //{
        //    try
        //    {
        //        WebRequest wr = WebRequest.Create("http://www.ip138.com/ip2city.asp");
        //        Stream s = wr.GetResponse().GetResponseStream();
        //        StreamReader sr = new StreamReader(s, Encoding.Default);
        //        string all = sr.ReadToEnd(); //读取网站的数据

        //        int start = all.IndexOf("[") + 1;
        //        int end = all.IndexOf("]", start);
        //        string tempip = all.Substring(start, end - start);
        //        sr.Close();
        //        s.Close();
        //        return tempip;
        //    }
        //    catch
        //    {
        //        return null;
        //    }
        //}
        public static string getLanIP()//获得局域网IP
        {
#if UNITY_EDITOR
            NetworkInterface[] nics = NetworkInterface.GetAllNetworkInterfaces();
            foreach (NetworkInterface adapter in nics)
            {
                if (adapter.Name == "WLAN")//判断是否是无线
                {
                    IPInterfaceProperties ip = adapter.GetIPProperties();     //IP配置信息
                    foreach (UnicastIPAddressInformation add in ip.UnicastAddresses)
                    {
                        if (add.Address.AddressFamily == AddressFamily.InterNetwork)
                            return add.Address.ToString();
                    }
                }
            }
#elif  UNITY_ANDROID
            IPAddress[] adds = Dns.GetHostEntry(Dns.GetHostName()).AddressList;
            foreach (IPAddress ip in adds)
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                    return ip.ToString();
#else
            
#endif
            return null;
        }
        //static string getInt64(int num)
        //{
        //    StringBuilder s = new StringBuilder();

        //    //一个int 是32位，要将其填充为36位，才可以按6位一组
        //    string num2 = Convert.ToString(num, 2).PadLeft(36, '0');//二进制

        //    for (short i = 0; i != 36; i += 6)//2的6次方，6位为一组
        //    {
        //        int index = 0;
        //        int times = 1;
        //        for (short k = (short)(i + 5); k > i - 1; --k)
        //        {
        //            index += (num2[k] - 48) * times;
        //            times *= 2;
        //        }
        //        s.Append(ASPECT_64[index]);
        //    }
        //    return s.ToString();
        //}
        //static int getInt64(string content)
        //{
        //    StringBuilder s = new StringBuilder();
        //    //一个int 转成的content一定有6个字符
        //    for (short i = 0; i != 6; i++)//将每个编码还原为数字，并转化为6位二进制
        //    {
        //        int index = ASPECT_64.FirstOrDefault(item => item.Value == content[i]).Key;
        //        string num2 = Convert.ToString(index, 2).PadLeft(6, '0');
        //        s.Append(num2);
        //    }

        //    return Convert.ToInt32(s.ToString(), 2);
        //}
    }
}