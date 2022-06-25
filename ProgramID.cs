using C3SharpInterface;
using C3SharpInterface.Requests;
using C3SharpInterface.Responses;
using Nancy.Json;
using RestSharp;
using System.Net;

namespace ProgramNummerCheck
{


    #region ReadDict

    public class DataLesen
    {
        public object DataOnRobot { get;  set; }

        public void ReadConfig(string txtFile1)
        {
            Variable.Variables DataLesen = new Variable.Variables();
            var dict = File.ReadAllLines(txtFile1)
                           .Select(l => l.Split(new[] { ';' }))
                           .ToDictionary(s => s[0].Trim(), s => s[1].Trim());  // read the entire file into a dictionary.

            DataOnRobot = Convert.ToString(dict[DataLesen.Name]);
            //ForTesting
            Console.WriteLine(dict[DataLesen.Name]);
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

            ValueOnRobot = Convert.ToInt32(dict[ValueLesen.Name]);
            //ForTesting
            Console.WriteLine("value:" + ValueOnRobot);
        }
    }
    #endregion

    public class ProgramNummer
    {
        private static int SubNr;
        private static string? ProgramName;
        private static float VersionOnRobot;

        static void Main(string[] args)
        {
           
           
                Console.WriteLine("Conecting");

            SyncClient syncClient = new SyncClient();
            syncClient.ConnectToHost(IPAddress.Parse(args.Length > 0 ? args[0] : "192.168.1.12"), 7000);  // if IP is different then we need to change here the nr. 


            for (; ; )
            {
                Subscription1();
                Subscription2();
                Subscription3();

                DataLesen dictLesen = new DataLesen();
                dictLesen.ReadConfig(@"C:\Users\marlena.knitter\Desktop\RoboSonic\RowaConsoleClient\ProgramDict.txt");
                ValueLesen valueLesen = new ValueLesen();
                valueLesen.ReadConfig(@"C:\Users\marlena.knitter\Desktop\RoboSonic\RowaConsoleClient\ValueDict.txt");
                // using variables 



                    Console.WriteLine(valueLesen.ValueOnRobot);

                // connection to robot

                Console.WriteLine("Connected");


                // do we need this data?

                string data = DateTime.Now.ToString("yyyy-MM-dd");

                // path to the file for variablen - when it will be on server then it will be to change
                // string file = @"C:\Users\marlena.knitter\Desktop\DataTest.txt";

                // Add dictionary 1 string - pr name, 2 string last modification datum 

                Dictionary<string, string> ProgramDict = new Dictionary<string, string>();
                Dictionary<string, int> ValueDict = new Dictionary<string, int>();

                Variable.Variables ProgramCheck = new Variable.Variables();
                ProgramDict.Add(ProgramName, Convert.ToString(dictLesen.DataOnRobot));
                // hier should be :
                // ProgramName = Name Variable from API 

                if (SubNr == 1)
                {
                    // hier starts the code, data for getting programm version starts here 
                    Console.WriteLine("Checking program name");
                    FilePropertiesRequest request26 = new FilePropertiesRequest(@"KRC:\R1\Program\ROWA\" + ProgramName);


                    Console.WriteLine("Program check sending request");
                    // 
                    FilePropertiesResponse response26 = (FilePropertiesResponse)syncClient.SendRequest(request26);
                    Console.WriteLine("Command: {0}, Success: {1}, ErrorCode: {2}", response26.Type, response26.Success, response26.ErrorCode);
                    Console.ReadLine();
                    if (response26.Success)
                    {

                        // need only this , later wee need only date ??


                        Console.WriteLine("    Name: {0}", response26.FileName);
                        Console.WriteLine("    Last Write Time: {0}", response26.LastWriteTime);

                        // VersDat is a string variable make from data we get from robot
                        string VersDat = Convert.ToString(response26.LastWriteTime);

                        string VersDatOnRobot = Convert.ToString(dictLesen.DataOnRobot);
                        Console.WriteLine(VersDatOnRobot);
                        Console.WriteLine(VersDat);
                        // check and compare date from file with data gotten from Robot

                        if (ProgramDict.ContainsKey(ProgramName) == true)
                        {



                            ProgramDict.TryGetValue(ProgramName, out VersDatOnRobot);
                            Console.WriteLine("old");

                        }

                        else
                        {
                            Console.WriteLine("");
                            ProgramDict.Add(ProgramName, VersDat);
                            ProgramCheck.VersionOnRobot = 1;
                            ValueDict.Add(ProgramName, ProgramCheck.VersionOnRobot);
                            VersDatOnRobot = VersDat;

                        }



                        if (VersDat != VersDatOnRobot)
                        {
                            Console.WriteLine("versdat != versonrobot");
                            ProgramCheck.VersionOnRobot = valueLesen.ValueOnRobot;
                            ProgramCheck.VersionOnRobot++;
                            VersDatOnRobot = VersDat;

                            ProgramDict[ProgramName] = VersDatOnRobot;
                            ValueDict[ProgramName] = ProgramCheck.VersionOnRobot;
                        }
                        else
                        {
                            ProgramCheck.VersionOnRobot = valueLesen.ValueOnRobot;
                            ValueDict[ProgramName] = ProgramCheck.VersionOnRobot;
                        }
                        // hier should be variable we are using for sending it away to fiware 

                        #region dict to file 
                        string filePath = @"C:\Users\marlena.knitter\Desktop\RoboSonic\RowaConsoleClient\ProgramDict.txt";
                        using (FileStream fs = new FileStream(filePath, FileMode.Append))
                        {
                            using (TextWriter tw = new StreamWriter(fs))

                                foreach (KeyValuePair<string, string> kvp in ProgramDict)
                                {
                                    tw.WriteLine(string.Format("{0};{1}", kvp.Key, kvp.Value));
                                }
                        }

                        string filePath2 = @"C:\Users\marlena.knitter\Desktop\RoboSonic\RowaConsoleClient\ValueDict.txt";
                        using (FileStream fs = new FileStream(filePath2, FileMode.Append))
                        {
                            using (TextWriter tw = new StreamWriter(fs))

                                foreach (KeyValuePair<string, int> kvp1 in ValueDict)
                                {
                                    tw.WriteLine(string.Format("{0};{1}", kvp1.Key, kvp1.Value));
                                }
                        }

                        #endregion


                        Console.WriteLine("Data we get:");
                        Console.WriteLine("Asked PRogram name:" + ProgramName);
                        Console.WriteLine("gotten from Robot ProgramName:" + response26.FileName);
                        Console.WriteLine("Version on robot: {0}", ProgramCheck.VersionOnRobot);
                        Console.WriteLine("VersDat:" + VersDat);
                        Console.WriteLine("VersDatOnRobot" + VersDatOnRobot);
                        Console.ReadLine();
                        Console.WriteLine("Checking programm");

                        VersionOnRobot = ProgramCheck.VersionOnRobot;
                        Answer();

                    }
                    //for program uploading to  server
                }

                if (SubNr == 2)
                {
                    SetFileAttributesRequest request20 = new SetFileAttributesRequest(@"KRC:\R1\Program\ROWA\" + ProgramName, ItemAttribute.None, ItemAttribute.None);
                    Console.WriteLine(request20);

                    Response response = syncClient.SendRequest(request20);

                    // Move
                    response = syncClient.SendRequest(new CopyFileRequest(@"KRC:\R1\Program\ROWA\" + ProgramName, @"E:\" + ProgramName + "-" + data));
                    Console.WriteLine("Command: {0}, Success: {1}, ErrorCode: {2}", response.Type, response.Success, response.ErrorCode);
                    Console.ReadLine();
                }




                if (SubNr == 3)
                {


                    // //code for program download, to here must be API subscription which says what program is to check - check if works or no interface fault
                    SetFileAttributesRequest request21 = new SetFileAttributesRequest(@"C:\Users\marlena.knitter\Desktop\ProgTest\" + ProgramName, ItemAttribute.None, ItemAttribute.None);

                    Response response1 = syncClient.SendRequest(request21);
                    // Move
                    response1 = syncClient.SendRequest(new CopyFileRequest(@"C:\Users\marlena.knitter\Desktop\ProgTest" + ProgramName, @"KRC:\R1\PROGRAM\ROWA\"));
                    Console.WriteLine("Command: {0}, Success: {1}, ErrorCode: {2}", response1.Type, response1.Success, response1.ErrorCode);
                    Console.ReadLine();

                }

                SubNr = 0;

            }   
          
        }


        

        public static void Subscription1()

        {
            var client = new RestClient("http://localhost:5011/");
            var request = new RestRequest("/Subscription1");
            var response = client.Execute(request);

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                string Sub1 = System.IO.File.ReadAllText(@"C:\Users\marlena.knitter\Desktop\RoboBox\RowaKukaKlientAlpha\RowaConsoleClient\Sub1.txt");

                string rawResponse = response.Content;

                if (Sub1 != rawResponse)
                {
                    Console.WriteLine(rawResponse);
                    DataSub1 datumDto = new JavaScriptSerializer().Deserialize<DataSub1>(rawResponse); ;
                    foreach (var item in datumDto.data)
                    {
                        Console.WriteLine("Co wyszlo:" + item.productID.value);
                        ProgramName = item.productID.value;


                    }
                   File.WriteAllTextAsync("Sub1.txt", rawResponse);
                    SubNr = 1;
                }
               
            }
            else
            {
                Console.Write("Sh*t happend");
            }

        }
        public static void Subscription2()

        {
            var client = new RestClient("http://localhost:5011/");
            var request = new RestRequest("/Subscription2");
            var response = client.Execute(request);

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                string Sub2 = System.IO.File.ReadAllText(@"C:\Users\marlena.knitter\Desktop\RoboBox\RowaKukaKlientAlpha\RowaConsoleClient\Sub2.txt");

                string rawResponse = response.Content;

                if (Sub2 != rawResponse)
                {
                    Console.WriteLine(rawResponse);
                    DataSub1 datumDto = new JavaScriptSerializer().Deserialize<DataSub1>(rawResponse); ;
                    foreach (var item in datumDto.data)
                    {
                        Console.WriteLine("Co wyszlo:" + item.productID.value);
                        ProgramName = item.productID.value;


                    }
                    File.WriteAllTextAsync("Sub3.txt", rawResponse);
                    SubNr = 2;
                }
                
            }
            else
            {
                Console.Write("Sh*t happend");
            }

        }
        public static void Subscription3()

        {
            var client = new RestClient("http://localhost:5011/");
            var request = new RestRequest("/Subscription3");
            var response = client.Execute(request);

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                string Sub3 = System.IO.File.ReadAllText(@"C:\Users\marlena.knitter\Desktop\RoboBox\RowaKukaKlientAlpha\RowaConsoleClient\Sub3.txt");

                string rawResponse = response.Content;

                if (Sub3 != rawResponse)
                {
                    Console.WriteLine(rawResponse);
                    DataSub1 datumDto = new JavaScriptSerializer().Deserialize<DataSub1>(rawResponse); ;
                    foreach (var item in datumDto.data)
                    {
                        Console.WriteLine("Co wyszlo:" + item.productID.value);
                        ProgramName = item.productID.value;


                    }
                    File.WriteAllTextAsync("Sub3.txt", rawResponse);
                    SubNr = 3;
                }
                
            }
            else
            {
                Console.Write("Sh*t happend");
            }

        }
        public class DataSub1
        {
            public List<DatumDto> data { get; set; }

        }
        public class DatumDto
        {
            public ProductID productID {get; set;}
        }
        public class ProductID
        {

            public string value { get; set; }
        }


        public static void Answer()
        {

            var client = new RestClient("http://localhost:1026/");
            var request = new RestRequest("v2/entities/Robot1/attrs/writeOrderstatus?type=Order", Method.Put);
            request.AddHeader("fiware-service", "robot_info");
            request.AddHeader("fiware-servicepath", "/demo");
            request.AddHeader("Content-Type", "application/json");
            var body = @"{ ""value"": [" + VersionOnRobot + @",""test"",""test2"",""test3"",""test4"",""test5""]," + "\n" +
            @"   ""type"": ""command""" + "\n" + @"}";
            request.AddParameter("application/json", body, ParameterType.RequestBody);

        }

    }

}









