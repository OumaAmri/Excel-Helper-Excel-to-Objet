using NLog;
using OPM_Automatisation_DailyReport.Model;
using System.Collections.Generic;

namespace OPM_Automatisation_DailyReport.Message
{   
    public class LogMessage
    {
        private static Logger Logger = LogManager.GetLogger("Rule");
        private static Logger LogOKServer = LogManager.GetLogger("LoggerRuleOKServer");
        private static Logger LogNOKPingNOK = LogManager.GetLogger("LoggerRuleNOKPingNOK");
        private static Logger LogNOKPingOK = LogManager.GetLogger("LoggerRuleNOKPingOK"); 

        #region OKServer
        public void OKServer(List<Server> servers)
        {
            string message = "";
            LogOKServer.Info("  Servers without problem : \n");
            foreach (var server in servers)
            {
                message = message +
                       "\nIP Address : " + server.OPCIP + " \n\n" +
                       "Site : " + server.Site + " \n" +
                       "+--------------------------------------------------------------+\n";
            }
            LogOKServer.Info("---------------------------------------");
            LogOKServer.Info("\n"+message);           
        }

        public void EmptyOKServer()
        {
            LogOKServer.Info("Zero OKServer found !");
        }
        #endregion

        #region NOK Server With Ping NOK
        public void NOKPingNOK(List<Server> serversBySite, string site)
        {
            string message = "";
            LogNOKPingNOK.Info("List of unreachable servers for site :  " + site + "\n");
            foreach (var server in serversBySite)
            {
                message = message +
    "\nLocation      :   " + server.LocationOfServer + "\n" +
    "OPC Server  :   " + server.OPCServer + "\n" +
    "Adresse IP   :   " + server.OPCIP + "\n" +
    "+--------------------------------------------------------------+\n";
            }
            LogNOKPingNOK.Info(message);
        }
        public void EmptyNOKPingNOK()
        {
            LogNOKPingNOK.Info("Zero unreachable servers found.");
        }
        #endregion

        #region NOK Server With Ping OK
        public void NOKPingOK(List<Server> serversBySite, string site)
        {
            string message = "";
            LogNOKPingOK.Info("List of reachable servers but with problem. For site :  " + site + "\n");
            foreach (var server in serversBySite)
            {
                message = message +
    "\nLocation      :   " + server.LocationOfServer + "\n" +
    "OPC Server  :   " + server.OPCServer + "\n" +
    "Adresse IP   :   " + server.OPCIP + "\n" +
    "+--------------------------------------------------------------+\n";
            }
            LogNOKPingOK.Info(message);
        }
        public void EmptyNOKPingOK()
        {
            LogNOKPingOK.Info("Zero reachable servers with problem found.");
        }
        #endregion

        #region Logs
        public void LogUnknownTag(Server server)
        {
            Logger.Warn("Unknown status for :\n"+
                "'" + server.OPCIP + "', site :" + server.Site+", Location : "+server.LocationOfServer+"\n");

        }
        public void LogError(string message)
        {
            Logger.Error(message);
        }
        public void LogWarn(string message)
        {
            Logger.Warn(message);
        }

        public void LogIpAddressNull(Server server)
        {
            Logger.Error("Address IP null or empty for server : \n"+
                "Location : "+server.LocationOfServer+"\n"+
                "OPC Server : "+server.OPCServer);
        }
        #endregion

    }
}
