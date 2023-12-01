using AitukBattlePass.Models;
using System.IO;
using System.Xml.Serialization;

namespace AitukBattlePass
{
    public class DBManager
    {
        public static string pluginName = "AitukBattlePass";

        public static AitukServer GetDB()
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(AitukServer));
            using (FileStream fs = new FileStream($"Plugins/{pluginName}/DataBase.xml", FileMode.OpenOrCreate))
            {
                AitukServer server = (AitukServer)xmlSerializer.Deserialize(fs);
                return server;
            }
        }

        public static void ServializeServer()
        {
            AitukServer server = new AitukServer();
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(AitukServer));
            using (FileStream fs = new FileStream($"Plugins/{pluginName}/DataBase.xml", FileMode.OpenOrCreate))
            {
                xmlSerializer.Serialize(fs, server);
            }
        }

        public static void Save(AitukServer server)
        {
            FileInfo fileInfo = new FileInfo($"Plugins/{pluginName}/DataBase.xml");
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(AitukServer));
            fileInfo.Delete();
            using (FileStream fs = new FileStream($"Plugins/{pluginName}/DataBase.xml", FileMode.OpenOrCreate))
            {
                xmlSerializer.Serialize(fs, server);
            }
        }
    }
}
