using OPM_Automatisation_DailyReport.Helper;
using OPM_Automatisation_DailyReport.Message;
using OPM_Automatisation_DailyReport.Model;
using OPM_Automatisation_DailyReport.Notification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;

namespace OPM_Automatisation_DailyReport.Application_Start
{
    public class Application
    {
        ExcelHelper ExcelHelper = new ExcelHelper();
        LogMessage logMessage = new LogMessage();
        EmailService emailService = new EmailService();
        List<Server> NOKWithPingOK = new List<Server>();
        List<Server> NOKWithPingNOK = new List<Server>();
        List<Server> OKServer = new List<Server>();
        List<string> listSite = new List<string>();
        List<string> listSiteForMiddelware = new List<string>();

        public void Start()
        {
            try
            {
                var servers = ExcelHelper.GetServersFromExcel();
                if (servers == null)
                {
                    logMessage.LogError("Problem list servers null ");
                }
                if (servers.Count == 0)
                {
                    logMessage.LogWarn("Zero server found  ");
                }
                else
                {
                    foreach (var server in servers)
                    {
                        try
                        {
                            #region NOK Status
                            if (server.Status == "NOK")
                            {
                                if (!String.IsNullOrEmpty(server.OPCIP))
                                {
                                    
                                    Ping ping = new Ping();
                                    PingReply pingReply = ping.Send(server.OPCIP);
                                    if (pingReply.Status == IPStatus.Success)
                                    {
                                        NOKWithPingOK.Add(server);
                                    }
                                    else
                                    {
                                        NOKWithPingNOK.Add(server);
                                    }
                                }
                                else
                                {
                                    logMessage.LogIpAddressNull(server);
                                }

                            }
                            #endregion

                            #region OK Status
                            else if (server.Status == "OK")
                            {
                                OKServer.Add(server);
                            }

                            #endregion

                            else
                            {
                                logMessage.LogUnknownTag(server);
                            }
                        }
                        catch (Exception ex)
                        {
                            logMessage.LogError("Exception for " + server.OPCIP + " : " + ex.Message + "\nInnerException : " + ex.InnerException.Message);
                        }
                    }

                    #region LogOKServer
                    if (OKServer != null && OKServer.Any())
                    {
                        logMessage.OKServer(OKServer);
                    }
                    else
                    {
                        logMessage.EmptyOKServer();
                    }
                    #endregion

                    #region LogNOKPingNOK

                    if (NOKWithPingNOK != null && NOKWithPingNOK.Any())
                    {
                        foreach (var server in NOKWithPingNOK)
                        {
                            listSite.Add(server.Site);
                        }
                        List<string> listSiteDistinct = listSite.Distinct().ToList();

                        foreach (string site in listSiteDistinct)
                        {
                            string responsable = NOKWithPingNOK.ToList().Where(s => s.Site == site).FirstOrDefault().Email;
                            List<Server> listServerBySite = NOKWithPingNOK.Where(s => s.Site == site).ToList();

                            //Log Unreachable servers
                            logMessage.NOKPingNOK(listServerBySite, site);

                            #region Send Email
                           
                            if (!String.IsNullOrEmpty(responsable))
                            {
                                emailService.SendMsgNOKWithPingNOK(responsable,
                               "Unreachable Servers for site : " + site,
                               EmailMessage.MessageNOKWithPingNOK(listServerBySite));
                            }
                            else
                            {
                                logMessage.LogError("Email Null or Empty for responsable of site : " + site);
                            }
                            #endregion
                        }
                    }
                    else
                    {
                        logMessage.EmptyNOKPingNOK();
                    }
                    #endregion

                    #region LogNOKPingOK
                    if (NOKWithPingOK != null && NOKWithPingOK.Any())
                    {
                        foreach (var server in NOKWithPingOK)
                        {
                            listSiteForMiddelware.Add(server.Site);
                        }
                        List<string> listSiteDistinctForMiddelware = listSiteForMiddelware.Distinct().ToList();

                        foreach (string site in listSiteDistinctForMiddelware)
                        {
                            List<Server> listServerBySiteForMiddelware = NOKWithPingOK.Where(s => s.Site == site).ToList();

                            //Log reachable servers with problem
                            logMessage.NOKPingOK(listServerBySiteForMiddelware, site);

                            #region Send Email
                           
                            emailService.SendMsgNOKWithPingOK("Reachable Servers with problem for site : " + site,
                            EmailMessage.MessageNOKWithPingOK(listServerBySiteForMiddelware));
                            #endregion
                        }
                    }
                    else
                    {
                        logMessage.EmptyNOKPingOK();
                    }
                    #endregion
                }
            }
            catch(Exception ex)
            {
                logMessage.LogError("Exception : " + ex.Message + "\nInnerEcxeption : " + ex.InnerException.Message);

            }

        }
    }
}
