using OfficeOpenXml;
using OPM_Automatisation_DailyReport.Message;
using OPM_Automatisation_DailyReport.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace OPM_Automatisation_DailyReport.Helper
{
    public class ExcelHelper
    {
        public string locationExcelFile = @"C:\Users\....\Desktop\OPCExcel.xlsx";

        LogMessage logMessage = new LogMessage();
        public List<Server> GetServersFromExcel()
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            var servers = new List<Server>();           
            {
                try
                {
                    FileInfo fi = new FileInfo(locationExcelFile);
                    using (var package = new ExcelPackage(fi))
                    {
                        var workSheet = package.Workbook.Worksheets.First();
                        var start = workSheet.Dimension.Start;
                        var end = workSheet.Dimension.End;

                        #region Vérifier que toutes les colonnes existent
                        {
                            var expectedColumnList = new List<string>(new[] {

                            "Location of server",
                            "OPC Server" ,
                            "Status"  ,                        
                            "OPC IP",
                            "Site",
                            "Responsible for Incident",
                            "Email",
                            });

                            var actualColumnList = new List<string>();
                            for (int col = start.Column; col <= end.Column; col++)
                            {
                                actualColumnList.Add(workSheet.Cells[1, col].Text);

                            }
                            foreach (var item in expectedColumnList)
                            {
                                if (!actualColumnList.Contains(item))
                                {
                                    logMessage.LogError("La colonne '" + item + "' est introuvable dans le fichier excel");                                  
                                }
                            }
                        }
                        #endregion

                        #region extraire la liste des serveurs
                        var columnIndexes = new Dictionary<string, int>();
                        for (int col = start.Column; col <= end.Column; col++)
                        {
                            columnIndexes.Add(workSheet.Cells[1, col].Text, col);
                        }
                        for (int row = start.Row + 1; row <= end.Row; row++)
                        {                            
                            Server server = new Server
                            {
                                LocationOfServer = workSheet.Cells[row, columnIndexes["Location of server"]].Text,
                                OPCServer = workSheet.Cells[row, columnIndexes["OPC Server"]].Text,
                                Status = workSheet.Cells[row, columnIndexes["Status"]].Text,                            
                                OPCIP = workSheet.Cells[row, columnIndexes["OPC IP"]].Text,
                                Site = workSheet.Cells[row, columnIndexes["Site"]].Text,
                                ResponsibleForIncident = workSheet.Cells[row, columnIndexes["Responsible for Incident"]].Text,
                                Email = workSheet.Cells[row, columnIndexes["Email"]].Text,
                            };

                            servers.Add(server);
                        }
                        #endregion
                    }
                }
                catch (Exception ex)
                {
                    logMessage.LogError("Exception : ExcelHelper =>  " + ex.Message + "\nInnerEcxeption : " + ex.InnerException.Message);
                }
            }
           
            return servers;
        }
    }
}
