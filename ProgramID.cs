using C3SharpInterface;
using C3SharpInterface.Requests;
using C3SharpInterface.Responses;
using Nancy.Json;
using RestSharp;
using System.Net;
using System.Text;

namespace ProgramNummerCheck
{


    #region ReadDict

    public class DataLesen
    {
        public object DataOnRobot { get; set; }

        public void ReadConfig(string txtFile1)
        {

            Variable.Variables DataLesen = new Variable.Variables();
            var dict = File.ReadAllLines(txtFile1)
                           .Select(l => l.Split(new[] { ';' }))
                           .ToDictionary(s => s[0].Trim(), s => s[1].Trim());  // read the entire file into a dictionary.

            try  // exception for new programms
            {
                DataOnRobot = Convert.ToString(dict[DataLesen.Name]);
            }
            catch (Exception ex)
            {

            }



        }

    }
    public class ValueLesen
    {
        public int ValueOnRobot { get; set; }

        public void ReadConfig(string txtFile1)
        {

            Variable.Variables ValueLesen = new Variable.Variables();
            var dict = File.ReadAllLines(txtFile1)
                           .Select(l => l.Split(new[] { ';' }))
                           .ToDictionary(s => s[0].Trim(), s => s[1].Trim());  // read the entire file into a dictionary.
            try // exception for new programms
            {
                ValueOnRobot = Convert.ToInt32(dict[ValueLesen.Name]);

            }
            catch (Exception ex)
            {

            }
        }
    }
    #endregion

    public class ProgramNummer
    {
        #region usedVariable
        private static int SubNr;
        private static string? IDName;
        private static string? OrderID;

        private static string? ProgrammName;
        private static string? deadline;
        private static string? finishedTime;
        private static string? orderStatus;
        private static int planParts;
        private static int prodParts;
        private static string? startTime;
        private static string? workingStation;

        private static int planCycleTime;
        private static int processingLength;
        private static float VersionOnRobot;
        #endregion

        static void Main(string[] args)
        {



            SyncClient syncClient = new SyncClient();
            syncClient.ConnectToHost(IPAddress.Parse(args.Length > 0 ? args[0] : "192.168.1.12"), 7000);  // if IP is different then we need to change it here. 




            for (; ; )

            {


                // using variables

                Subscription1();
                Subscription2();
                Subscription3();



                DataLesen dictLesen = new DataLesen();
                dictLesen.ReadConfig(@"C:\Users\marlena.knitter\Desktop\RoboSonic\RowaConsoleClient\ProgramDict.txt");
                ValueLesen valueLesen = new ValueLesen();
                valueLesen.ReadConfig(@"C:\Users\marlena.knitter\Desktop\RoboSonic\RowaConsoleClient\ValueDict.txt");



                string data = DateTime.Now.ToString("yyyy-MM-dd");



                Dictionary<string, string> ProgramDict = new Dictionary<string, string>();
                Dictionary<string, int> ValueDict = new Dictionary<string, int>();

                Variable.Variables ProgramCheck = new Variable.Variables();
                try
                {
                    ProgramDict.Add(ProgrammName, Convert.ToString(dictLesen.DataOnRobot));
                }
                catch (Exception ex)
                {
                    ProgramDict.Add("0", data);

                }


                if (SubNr == 1)
                {

                    FilePropertiesRequest request26 = new FilePropertiesRequest(@"KRC:\R1\Program\ROWA\" + ProgrammName);



                    // 
                    FilePropertiesResponse response26 = (FilePropertiesResponse)syncClient.SendRequest(request26);
                    if (response26.Success)
                    {


                        // VersDat is a string variable make from data we get from robot
                        string VersDat = Convert.ToString(response26.LastWriteTime);

                        string VersDatOnRobot = Convert.ToString(dictLesen.DataOnRobot);


                        if (ProgramDict.ContainsKey(ProgrammName) == true)
                        {



                            ProgramDict.TryGetValue(ProgrammName, out VersDatOnRobot);

                        }

                        else
                        {
                            ProgramDict.Add(ProgrammName, VersDat);
                            ProgramCheck.VersionOnRobot = 1;
                            ValueDict.Add(ProgrammName, ProgramCheck.VersionOnRobot);
                            VersDatOnRobot = VersDat;

                        }



                        if (VersDat != VersDatOnRobot)
                        {
                            ProgramCheck.VersionOnRobot = valueLesen.ValueOnRobot;
                            ProgramCheck.VersionOnRobot++;
                            VersDatOnRobot = VersDat;

                            ProgramDict[ProgrammName] = VersDatOnRobot;
                            ValueDict[ProgrammName] = ProgramCheck.VersionOnRobot;
                        }
                        else
                        {
                            ProgramCheck.VersionOnRobot = valueLesen.ValueOnRobot;
                            ValueDict[ProgrammName] = ProgramCheck.VersionOnRobot;
                        }


                        #region dict to file 
                        string lines = System.IO.File.ReadAllText(@"C:\Users\marlena.knitter\Desktop\RoboSonic\RowaConsoleClient\ProgramDict.txt");

                        if (lines.Contains(ProgrammName))
                        {
                            {
                                lines = lines.Replace(ProgrammName + ";" + dictLesen.DataOnRobot, ProgrammName + ";" + VersDatOnRobot);

                            }
                            File.WriteAllText(@"C:\Users\marlena.knitter\Desktop\RoboSonic\RowaConsoleClient\ProgramDict.txt", lines);
                        }
                        else
                        {

                            string filePath = @"C:\Users\marlena.knitter\Desktop\RoboSonic\RowaConsoleClient\ProgramDict.txt";
                            using (FileStream fs = new FileStream(filePath, FileMode.Append))
                            {
                                using (TextWriter tw = new StreamWriter(fs))

                                    foreach (KeyValuePair<string, string> kvp in ProgramDict)
                                    {

                                        tw.WriteLine(string.Format("{0};{1}", kvp.Key, kvp.Value));

                                    }
                            }

                        }
                        string lines2 = System.IO.File.ReadAllText(@"C:\Users\marlena.knitter\Desktop\RoboSonic\RowaConsoleClient\ValueDict.txt");

                        if (lines2.Contains(ProgrammName))
                        {
                            {
                                lines2 = lines2.Replace(ProgrammName + ";" + valueLesen.ValueOnRobot, ProgrammName + ";" + ProgramCheck.VersionOnRobot);

                            }
                            File.WriteAllText(@"C:\Users\marlena.knitter\Desktop\RoboSonic\RowaConsoleClient\ValueDict.txt", lines2);
                        }
                        else
                        {
                            string filePath2 = @"C:\Users\marlena.knitter\Desktop\RoboSonic\RowaConsoleClient\ValueDict.txt";
                            using (FileStream fs = new FileStream(filePath2, FileMode.Append))
                            {
                                using (TextWriter tw = new StreamWriter(fs))

                                    foreach (KeyValuePair<string, int> kvp1 in ValueDict)
                                    {
                                        tw.WriteLine(string.Format("{0};{1}", kvp1.Key, kvp1.Value));
                                    }
                            }

                        }

                        #endregion


                        //Console.WriteLine("Data we get:");
                        //Console.WriteLine("Asked PRogram name:" + ProgrammName);
                        //Console.WriteLine("gotten from Robot ProgramName:" + response26.FileName);
                        //Console.WriteLine("Version on robot: {0}", ProgramCheck.VersionOnRobot);
                        //Console.WriteLine("VersDat:" + VersDat);
                        //Console.WriteLine("VersDatOnRobot" + VersDatOnRobot);
                        //Console.WriteLine("Checking programm");

                        VersionOnRobot = ProgramCheck.VersionOnRobot;
                        Answer();
                        SubNr = 0;
                    }
                }

                if (SubNr == 2)
                {


                    SetFileAttributesRequest request20 = new SetFileAttributesRequest(@"KRC:\R1\Program\ROWA\" + ProgrammName, ItemAttribute.None, ItemAttribute.None);

                    Response response = syncClient.SendRequest(request20);

                    // Move
                    response = syncClient.SendRequest(new CopyFileRequest(@"KRC:\R1\Program\ROWA\" + ProgrammName, @"E:\" + ProgrammName + "-" + data));

                    SubNr = 0;
                }




                if (SubNr == 3)
                {

                    SetFileAttributesRequest request21 = new SetFileAttributesRequest(@"E:\" + ProgrammName, ItemAttribute.None, ItemAttribute.None);

                    Response response1 = syncClient.SendRequest(request21);

                    // Move
                    response1 = syncClient.SendRequest(new CopyFileRequest(@"E:\" + ProgrammName, @"KRC:\R1\PROGRAM\ROWA\" + ProgrammName));


                    SubNr = 0;

                }




            }
        }




        public static void Subscription1()

        {

            var client = new RestClient("http://localhost:5011/");
            var request = new RestRequest("/Subscription1");
            var response = client.Execute(request);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {


                string Sub1 = System.IO.File.ReadAllText(@"C:\Users\marlena.knitter\Desktop\RoboSonic\RowaConsoleClient\Sub1.txt");

                string rawResponse = response.Content;

                if (Sub1 != rawResponse)
                {
                    try
                    {
                        DataSub1 datumDto = new JavaScriptSerializer().Deserialize<DataSub1>(rawResponse); ;
                        foreach (var item in datumDto.data)
                        {
                            Console.WriteLine("coJest" + item.productID.value);
                            IDName = item.productID.value;

                        }

                        File.WriteAllTextAsync(@"C:\Users\marlena.knitter\Desktop\RoboSonic\RowaConsoleClient\Sub1.txt", rawResponse);
                        SubNr = 1;
                        GetDataID();

                    }
                    catch (Exception ex)

                    {
                    }
                }

            }
            else
            {
                Console.Write("Error occured");
            }

        }
        public static void Subscription2()

        {
            var client = new RestClient("http://localhost:5011/");
            var request = new RestRequest("/Subscription2");
            var response = client.Execute(request);

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                string Sub2 = System.IO.File.ReadAllText(@"C:\Users\marlena.knitter\Desktop\RoboSonic\RowaConsoleClient\Sub2.txt");

                string rawResponse = response.Content;

                if (Sub2 != rawResponse)
                {

                    try
                    {
                        DataSub1 datumDto = new JavaScriptSerializer().Deserialize<DataSub1>(rawResponse); ;
                        foreach (var item in datumDto.data)
                        {

                            IDName = item.productID.value;


                        }
                        File.WriteAllTextAsync(@"C:\Users\marlena.knitter\Desktop\RoboSonic\RowaConsoleClient\Sub2.txt", rawResponse);
                        SubNr = 2;
                        GetDataID();
                    }
                    catch (Exception ex)

                    {
                    }
                }

            }
            else
            {
                Console.Write("Error occured");
            }

        }
        public static void Subscription3()

        {
            var client = new RestClient("http://localhost:5011/");
            var request = new RestRequest("/Subscription3");
            var response = client.Execute(request);

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                string Sub3 = System.IO.File.ReadAllText(@"C:\Users\marlena.knitter\Desktop\RoboSonic\RowaConsoleClient\Sub3.txt");

                string rawResponse = response.Content;

                if (Sub3 != rawResponse)
                {

                    try
                    {
                        DataSub1 datumDto = new JavaScriptSerializer().Deserialize<DataSub1>(rawResponse); ;
                        foreach (var item in datumDto.data)
                        {

                            IDName = item.productID.value;

                        }
                        File.WriteAllTextAsync(@"C:\Users\marlena.knitter\Desktop\RoboSonic\RowaConsoleClient\Sub3.txt", rawResponse);
                        SubNr = 3;
                        GetDataID();
                    }
                    catch (Exception ex)

                    {
                    }
                }

            }
            else
            {
                Console.Write("Sh*t happend");
            }

        }

        public static void GetDataID()

        {

            Console.WriteLine("Co wyszlo:" + IDName);
            var client = new RestClient("http://localhost:1026/");
            var request = new RestRequest("/v2/entities/" + IDName);
            request.AddHeader("fiware-service", "robot_info");
            request.AddHeader("fiware-servicepath", "/demo");
            var response = client.Execute(request);
            if (IDName != null)
            {
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    //    string Sub1 = System.IO.File.ReadAllText(@"C:\Users\marlena.knitter\Source\Repos\CSharpApp-\test.txt");

                    string rawResponse1 = response.Content;

                    //if (Sub1 != rawResponse)
                    //{
                    Console.WriteLine("poszlo");
                    Console.WriteLine(rawResponse1);
                    IdName itemID = new JavaScriptSerializer().Deserialize<IdName>(rawResponse1);


                    OrderID = itemID.productID.value;
                    deadline = itemID.deadline.value;
                    finishedTime = itemID.finishedTime.value;
                    orderStatus = itemID.orderStatus.value;
                    planParts = itemID.planParts.value;
                    prodParts = itemID.prodParts.value;
                    startTime = itemID.startTime.value;
                    workingStation = itemID.workingStation.value;
                    //Console.WriteLine("order:{0}, deadline:{1}, finishedTime:{2}, orderStatus:{3}, planParts:{4}, prodParts :{5}, StartTime:{6}, workSt:{7} ", OrderID, deadline, finishedTime, orderStatus, planParts, prodParts, startTime, workingStation);
                    GetProgramData();
                }
                else
                {
                    Console.Write("Sh*t happend");
                    Console.WriteLine("pok  PR NAME: " + ProgrammName);
                    if (ProgrammName == null)
                        SubNr = 0;
                }
            }



        }



        public static void GetProgramData()

        {

            Console.WriteLine("Co wyszlo:" + OrderID);
            var client = new RestClient("http://localhost:1026/");
            var request = new RestRequest("/v2/entities/" + OrderID);
            request.AddHeader("fiware-service", "robot_info");
            request.AddHeader("fiware-servicepath", "/demo");
            var response = client.Execute(request);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                //    string Sub1 = System.IO.File.ReadAllText(@"C:\Users\marlena.knitter\Source\Repos\CSharpApp-\test.txt");

                string rawResponse2 = response.Content;

                //if (Sub1 != rawResponse)
                //{
                Console.WriteLine("poszlo");
                Console.WriteLine(rawResponse2);
                ProductName programName = new JavaScriptSerializer().Deserialize<ProductName>(rawResponse2);
                try
                {

                    ProgrammName = programName.programName.value;
                    planCycleTime = Convert.ToInt32(programName.planCycleTime.value);
                    processingLength = Convert.ToInt32(programName.processingLength.value);


                    Console.WriteLine("programName:{0}, PlanCycTime:{1}, procLenght:{2}", ProgrammName, planCycleTime, processingLength);



                }
                catch (NullReferenceException ex)
                {
                }
            }
            else
            {
                Console.Write("Sh*t happend");

            }





        }
        #region variable

        public class DataSub1
        {
            public List<DatumDto> data { get; set; }

        }
        public class DatumDto
        {
            public ProductID productID { get; set; }
        }
        public class ProductID
        {

            public string value { get; set; }
        }

        public class IdName
        {

            public ItemID productID { get; set; }
            public Deadline deadline { get; set; }
            public FinishedTime finishedTime { get; set; }
            public OrderStatus orderStatus { get; set; }
            public PlanParts planParts { get; set; }
            public ProdParts prodParts { get; set; }
            public StartTime startTime { get; set; }
            public WorkingStation workingStation { get; set; }

        }

        public class ItemID
        {
            public string value { get; set; }
        }

        public class Deadline
        {
            public string value { get; set; }
        }
        public class FinishedTime
        {
            public string value { get; set; }
        }
        public class OrderStatus
        {
            public string value { get; set; }
        }
        public class PlanParts
        {
            public int value { get; set; }
        }
        public class ProdParts
        {
            public int value { get; set; }
        }
        public class StartTime
        {
            public string value { get; set; }
        }
        public class WorkingStation
        {
            public string value { get; set; }
        }


        public class ProductName
        {

            public ProgramName programName { get; set; }
            public PlanCycleTime planCycleTime { get; set; }
            public ProcessingLength processingLength { get; set; }


        }

        public class ProgramName
        {
            public string value { get; set; }
        }
        public class PlanCycleTime
        {
            public int value { get; set; }
        }
        public class ProcessingLength
        {
            public int value { get; set; }
        }
        #endregion

        public static void Answer()
        {

            var client = new RestClient("http://localhost:1026/");
            var request = new RestRequest("v2/entities/Robot1/attrs/writeOrderstatus?type=Order", Method.Put);
            request.AddHeader("fiware-service", "robot_info");
            request.AddHeader("fiware-servicepath", "/demo");
            request.AddHeader("Content-Type", "application/json");
          

            var body = @"{" + "\n" + @"""value"": [""" + orderStatus + @""", """ + IDName + @""", """ + ProgrammName + @""", """ + OrderID + @""",  " + VersionOnRobot + @", " + planCycleTime + @", " + planCycleTime + @", " + planParts + "]," + "\n" +
            @"   ""type"": ""command""" + "\n" + @"}";
            string bod1 = Convert.ToString(body); 

            Console.WriteLine(body);

            request.AddJsonBody(bod1);

            client.Execute(request);


        }

    }
}











