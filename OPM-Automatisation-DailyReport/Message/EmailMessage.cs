using OPM_Automatisation_DailyReport.Model;
using System.Collections.Generic;

namespace OPM_Automatisation_DailyReport.Message
{  
    public class EmailMessage
    {
        //Body des emails
        public static string MessageNOKWithPingNOK(List<Server> serveurs)
        {
            string message = "";
            string msg1 = "Hello,\n\n" +
"Please find the list of unreachable servers :\n\n";
            foreach (var server in serveurs)
            {
                message = message +

    "Location      :   " + server.LocationOfServer + "\n" +
    "OPC Server  :   " + server.OPCServer + "\n" +
    "Adresse IP   :   " + server.OPCIP + "\n" +
    "Site              :   " + server.Site + "\n\n" +
    "+--------------------------------------------------------------+\n\n";
            }
            string msg3 = "Could you, please, fix this problem?\n\n" +
  "Best Regards,\n" +
  "Automation System";


            return msg1 + message + msg3;
        }
        public static string MessageNOKWithPingOK(List<Server> serveurs)
        {
            string message = "";
            string msg1 = "Hello,\n\n" +
"Please find the list of reachable servers with problem:\n\n";
            foreach (var server in serveurs)
            {
                message = message +

    "Location      :   " + server.LocationOfServer + "\n" +
    "OPC Server  :   " + server.OPCServer + "\n" +  
    "Adresse IP   :   " + server.OPCIP + "\n" +
    "Site              :   " + server.Site + "\n\n" +
    "+--------------------------------------------------------------+\n\n";
            }
            string msg3 = "Could you, please, fix this problem?\n\n" +
  "Best Regards,\n" +
  "Automation System";


            return msg1 + message + msg3;
        }
    }
}
