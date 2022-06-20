using C3SharpInterface;
using C3SharpInterface.Requests;
using C3SharpInterface.Responses;
using System.Net;

namespace ProgramNummerCheck
{
    #region ReadDict

    public class DataLesen
    {



        public void ReadConfig(string txtFile1)
        {
            Variable.Variables ProgramCheck = new Variable.Variables();
            var dict = File.ReadAllLines(txtFile1)
                           .Select(l => l.Split(new[] { ';' }))
                           .ToDictionary(s => s[0].Trim(), s => s[1].Trim());  // read the entire file into a dictionary.

            //ForTesting
            //  Console.WriteLine(dict[ProgramCheck.Name]);
        }

    }
    public class ValueLesen
    {



        public void ReadConfig(string txtFile1)
        {
            Variable.Variables ProgramCheck = new Variable.Variables();
            var dict = File.ReadAllLines(txtFile1)
                           .Select(l => l.Split(new[] { ';' }))
                           .ToDictionary(s => s[0].Trim(), s => s[1].Trim());  // read the entire file into a dictionary.

            //ForTesting
            //Console.WriteLine(dict[ProgramCheck.Name]);
        }
    }
    #endregion

    public class ProgramNummer
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Conecting");


            DataLesen dictLesen = new DataLesen();
            dictLesen.ReadConfig(@"C:\Users\marlena.knitter\Desktop\RoboSonic\RowaConsoleClient\ProgramDict.txt");
            ValueLesen dictLesen2 = new ValueLesen();
            dictLesen.ReadConfig(@"C:\Users\marlena.knitter\Desktop\RoboSonic\RowaConsoleClient\ValueDict.txt");


            // connection to robot

            SyncClient syncClient = new SyncClient();
            syncClient.ConnectToHost(IPAddress.Parse(args.Length > 0 ? args[0] : "192.168.1.12"), 7000);  // if IP is different then we need to change here the nr. 



            Console.WriteLine("Connected");


            // do we need this data?

            string data = DateTime.Now.ToString("yyyy-MM-dd");

            // path to the file for variablen - when it will be on server then it will be to change
            // string file = @"C:\Users\marlena.knitter\Desktop\DataTest.txt";

            // Add dictionary 1 string - pr name, 2 string last modification datum 

            Dictionary<string, string> ProgramDict = new Dictionary<string, string>();
            Dictionary<string, int> ValueDict = new Dictionary<string, int>();

            // using variables 
            Variable.Variables ProgramCheck = new Variable.Variables();
            // hier should be :
            // ProgramCheck.Name = Name Variable from API 


            // hier starts the code, data for getting programm version starts here 
            Console.WriteLine("Checking program name");
            FilePropertiesRequest request26 = new FilePropertiesRequest(@"KRC:\R1\Program\ROWA\" + ProgramCheck.Name);


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

                string? VersDatOnRobot;

                // check and compare date from file with data gotten from Robot

                if (ProgramDict.ContainsKey(ProgramCheck.Name) == true)
                {


                    ProgramDict.TryGetValue(ProgramCheck.Name, out VersDatOnRobot);
                    Console.WriteLine("old");

                }

                else
                {

                    ProgramDict.Add(ProgramCheck.Name, VersDat);
                    ProgramCheck.VersionOnRobot = 1;
                    ValueDict.Add(ProgramCheck.Name, ProgramCheck.VersionOnRobot);
                    VersDatOnRobot = VersDat;
                }



                if (VersDat != VersDatOnRobot)
                {
                    Console.WriteLine("versdat != versonrobot");

                    VersDatOnRobot = VersDat;

                    ProgramDict[ProgramCheck.Name] = VersDatOnRobot;
                    ValueDict[ProgramCheck.Name] = ProgramCheck.VersionOnRobot;
                }
                // hier should be variable we are using for sending it away to fiware 

                #region dict to file 
                string filePath = @"C:\Users\marlena.knitter\Desktop\RoboSonic\RowaConsoleClient\ProgramDict.txt";
                using (FileStream fs = new FileStream(filePath, FileMode.OpenOrCreate))
                {
                    using (TextWriter tw = new StreamWriter(fs))

                        foreach (KeyValuePair<string, string> kvp in ProgramDict)
                        {
                            tw.WriteLine(string.Format("{0};{1}", kvp.Key, kvp.Value));
                        }
                }

                string filePath2 = @"C:\Users\marlena.knitter\Desktop\RoboSonic\RowaConsoleClient\ValueDict.txt";
                using (FileStream fs = new FileStream(filePath2, FileMode.OpenOrCreate))
                {
                    using (TextWriter tw = new StreamWriter(fs))

                        foreach (KeyValuePair<string, int> kvp1 in ValueDict)
                        {
                            tw.WriteLine(string.Format("{0};{1}", kvp1.Key, kvp1.Value));
                        }
                }

                #endregion


                Console.WriteLine("Data we get:");
                Console.WriteLine("Asked PRogram name:" + ProgramCheck.Name);
                Console.WriteLine("gotten from Robot ProgramName:" + response26.FileName);
                Console.WriteLine("Version on robot: {0}", ProgramCheck.VersionOnRobot);
                Console.WriteLine("VersDat:" + VersDat);
                Console.WriteLine("VersDatOnRobot" + VersDatOnRobot);
                Console.ReadLine();
                Console.WriteLine("Checking programm");


                //for program uploading to  server


                //Console.WriteLine("Request succes");
                // SetFileAttributesRequest request20 = new SetFileAttributesRequest(@"KRC:\R1\Program\ROWA\" + ProgramCheck.Name, ItemAttribute.None, ItemAttribute.None);
                // Console.WriteLine(request20);

                // Response response = syncClient.SendRequest(request20);

                // // Move
                // response = syncClient.SendRequest(new CopyFileRequest(@"KRC:\R1\Program\ROWA\" + ProgramCheck.Name, @"E:\" + ProgramCheck.Name + "-" + data));
                // Console.WriteLine("Command: {0}, Success: {1}, ErrorCode: {2}", response.Type, response.Success, response.ErrorCode);
                // Console.ReadLine();

                // Console.WriteLine("Request Failed");





                // //code for program download, to here must be API subscription which says what program is to check - check if works or no interface fault
                // SetFileAttributesRequest request21 = new SetFileAttributesRequest(@"C:\Users\marlena.knitter\Desktop\ProgTest\" + ProgramCheck.Name, ItemAttribute.None, ItemAttribute.None);

                // Response response1 = syncClient.SendRequest(request21);
                // // Move
                // response1 = syncClient.SendRequest(new CopyFileRequest(@"C:\Users\marlena.knitter\Desktop\ProgTest" + ProgramCheck.Name , @"KRC:\R1\PROGRAM\ROWA\" ));
                // Console.WriteLine("Command: {0}, Success: {1}, ErrorCode: {2}", response.Type, response.Success, response.ErrorCode);
                // Console.ReadLine();
            }


        }


    }

}









