using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Converter_Smeta_BusGov
{
    class Convert
    {
        public static string Converter(string[] lines, string nameXmlFile, string fileShema)
        {

            List<reportItemF0503721TopLevelType2015> smetaIncome = new List<reportItemF0503721TopLevelType2015>();
            List<reportItemF0503721TopLevelType2015> smetaExpense = new List<reportItemF0503721TopLevelType2015>();
            List<reportItemF0503721TopLevelType2015> smetaNonFinancialAssets = new List<reportItemF0503721TopLevelType2015>();
            List<reportItemF0503721TopLevelType2015> smetaFinancialAssets = new List<reportItemF0503721TopLevelType2015>();
    
            int i = 0;
            while (i < lines.Length)
            {
                string[] arrayData = lines[i].Replace(".", ",").Split('|');
                if (arrayData.Length == 7 && Int32.TryParse(arrayData[0], out int kodStr)) 
                {
                    reportItemF0503721TopLevelType2015 item = new reportItemF0503721TopLevelType2015()
                    {
                        name = ReciveNameKosgu.FromNumber(arrayData[1]),
                        lineCode = arrayData[0],
                        analyticCode = arrayData[1].Contains("***") ? "X" : arrayData[1],
                        targetFunds = Decimal.Parse(arrayData[2]),
                        targetFundsSpecified = true,
                        stateTaskFunds = Decimal.Parse(arrayData[3]),
                        stateTaskFundsSpecified = true,
                        revenueFunds = Decimal.Parse(arrayData[4]),
                        revenueFundsSpecified = true,
                        total = Decimal.Parse(arrayData[5]),
                        totalSpecified = true,
                    };
                    List<reportItemF0503721BaseType2015> reportSubitem = new List<reportItemF0503721BaseType2015>();
                    while (lines[i + 1].StartsWith(arrayData[0]))
                    {
                        i++;
                        string[] arrayDataSubitem = lines[i].Replace(".", ",").Split('|');
                        reportItemF0503721BaseType2015 subItem = new reportItemF0503721BaseType2015() 
                        {
                            name = ReciveNameKosgu.FromNumber(arrayDataSubitem[1]),
                            lineCode = arrayDataSubitem[0],
                            analyticCode = arrayDataSubitem[1].Contains("***") ? "X" : arrayDataSubitem[1],
                            targetFunds = Decimal.Parse(arrayDataSubitem[2]),
                            targetFundsSpecified = true,
                            stateTaskFunds = Decimal.Parse(arrayDataSubitem[3]),
                            stateTaskFundsSpecified = true,
                            revenueFunds = Decimal.Parse(arrayDataSubitem[4]),
                            revenueFundsSpecified = true,
                            total = Decimal.Parse(arrayDataSubitem[5]),
                            manually = true,
                            totalSpecified = true,
                        };
                        reportSubitem.Add(subItem);
                    }

                    if (reportSubitem.Count > 0) item.reportSubItem = reportSubitem.ToArray();

                    if (kodStr < 150) smetaIncome.Add(item);
                    if (kodStr >= 150 && kodStr < 310) smetaExpense.Add(item);
                    if (kodStr >= 310 && kodStr < 400) smetaNonFinancialAssets.Add(item);
                    if (kodStr >= 400) smetaFinancialAssets.Add(item);
                }
                i++;
            }
            var emptyItem = new reportItemF0503721TopLevelType2015()
            {
                name = "0",
                lineCode = "0",
                analyticCode = "0",
            };

            annualBalanceF0503721_2015 f721 = new annualBalanceF0503721_2015()
            {
                header = new headerType
                {
                    id = Guid.NewGuid().ToString(),
                    createDateTime = DateTime.Now,
                },
                body = new annualBalanceF0503721_2015Body
                {
                    position = new annualBalanceF0503721Type_2015()
                    {
                        positionId = Guid.NewGuid().ToString(),
                        changeDate = DateTime.Now,
                        generalData = new annualBalanceFounderDataType_2014()
                        {
                            date = new DateTime(2021, 01, 01),
                            periodicity = periodicityType.annual,
                            okei = new refNsiOkeiType() { code = "383" },
                            okpo = "00000000",
                            inn = "0000000000",
                            oktmo = new refNsiOktmoType() { code = "00000000" },
                            founderAuthority = new refNsiConsRegSoftType() { fullName = "Администрация" },
                        },
                        income = smetaIncome.ToArray().Length > 0 ? 
                        smetaIncome.ToArray() : new reportItemF0503721TopLevelType2015[]
                        {emptyItem},
                        expense = smetaExpense.ToArray().Length > 0 ?
                        smetaExpense.ToArray() : new reportItemF0503721TopLevelType2015[]
                        {emptyItem},
                        nonFinancialAssets = smetaNonFinancialAssets.ToArray().Length > 0 ?
                        smetaNonFinancialAssets.ToArray() : new reportItemF0503721TopLevelType2015[]
                        {emptyItem},
                        financialAssets = smetaFinancialAssets.ToArray().Length > 0 ?
                        smetaFinancialAssets.ToArray() : new reportItemF0503721TopLevelType2015[]
                        {emptyItem},
                    },
                },
            };

            XmlSerializer serializer = new XmlSerializer(typeof(annualBalanceF0503721_2015));
            FileStream fs = new FileStream(nameXmlFile, FileMode.Create);
            serializer.Serialize(fs, f721);
            fs.Close();

            //Валидация
            XmlSchemaSet schemas = new XmlSchemaSet();
            schemas.Add(null, fileShema);
            XDocument document = XDocument.Load(nameXmlFile);
            string message = $"Файл {nameXmlFile} обработан без ошибок.";
            document.Validate(schemas, (sender, validationEventArgs) =>
            {
                //сообщение об ошибке валидации, если ошибок нет сообщение пустое
                message = $"Файл {nameXmlFile} обработан содержит ошибку: {validationEventArgs.Message}";
            });
            return message;
        }
    }
}
