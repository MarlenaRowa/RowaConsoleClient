using System;
using System.Net;
using C3SharpInterface;
using C3SharpInterface.Requests;
using C3SharpInterface.Responses;

namespace ProgramNummerCheck
{  
    
    
    public class Variable
    {
        public string Name { get; set; }
        public int VersionOnRobot  { get; set; }
    }


    public class ProgramNummer
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Conecting");

            // connection to robot

            SyncClient syncClient = new SyncClient();
            syncClient.ConnectToHost(IPAddress.Parse(args.Length > 0 ? args[0] : "198.162.1.12"), 7000);  // if IP is different then we need to change here the nr. 



            Console.WriteLine("Connected");


            // do we need this data?

            string data = DateTime.Now.ToString("yyyy-MM-dd");

            // path to the file for variablen - when it will be on server then it will be to change
            string file = @"C:\Users\marlena.knitter\Desktop\DataTest.txt";

            // Add dictionary 1 string - pr name, 2 string last modification datum 

            Dictionary<string, string> ProgramDict = new Dictionary<string, string>();

            // using variables 
            ProgramNummerCheck.Variable ProgramCheck = new ProgramNummerCheck.Variable();
            // hier should be :
            // ProgramCheck.Name = Name Variable from API 



            // hier starts the code, data for getting programm version starts here 
            Console.WriteLine("Checking program name");
            FilePropertiesRequest request26 = new FilePropertiesRequest(@"KRC:\R1\Program\ROWA" + ProgramCheck.Name);


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

                if (ProgramDict.ContainsValue(response26.FileName) == true)
                {
                    
                    ProgramDict.TryGetValue(response26.FileName, out VersDatOnRobot);
                }
                else
                {
                    ProgramDict.Add(response26.FileName, VersDat);
                    VersDatOnRobot = VersDat;
                }


                if (VersDat != VersDatOnRobot)
                {
                    ProgramCheck.VersionOnRobot++;

                    VersDatOnRobot = VersDat;

                    ProgramDict[response26.FileName] = VersDatOnRobot;

                }
             // hier should be variable we are using for sending it away to fiware 

            }

      
            // 

            // read the file, make a request


        if (syncClient.SendRequest(new CreateFileRequest(@"KRC:\R1\PROGRAM\" + ProgramCheck.Name)).Success)
        {
            SetFileAttributesRequest request20 = new SetFileAttributesRequest(@"KRC:/R1/PROGRAM/" + ProgramCheck.Name, ItemAttribute.ReadOnly, ItemAttribute.ReadOnly); // nie wiem czy dziala, sprawdzic potem! jak nie to wykobinowac by byly dwa
            Response response = syncClient.SendRequest(request20);

            // Move
            response = syncClient.SendRequest(new CopyFileRequest(@"KRC:\R1\PROGRAM\" + ProgramCheck.Name, @"C:\Users\marlena.knitter\Desktop\ProgTest" + ProgramCheck.Name + data, true));
            Console.WriteLine("Command: {0}, Success: {1}, ErrorCode: {2}", response.Type, response.Success, response.ErrorCode);
            Console.ReadLine();
        }



            // code for program download, to here must be API subscription which says what program is to check - check if works or no interface fault 
            if (syncClient.SendRequest(new CreateFileRequest(@"C:\Users\marlena.knitter\Desktop\RoboBox\TESTPROGRAM\COPY\" + ProgramCheck.Name)).Success)
            { 
                SetFileAttributesRequest request20 = new SetFileAttributesRequest(@"C:\Users\marlena.knitter\Desktop\RoboBox\TESTPROGRAM\COPY\test" + ProgramCheck.Name, ItemAttribute.ReadOnly, ItemAttribute.ReadOnly); // nie wiem czy dziala, sprawdzic potem! jak nie to wykobinowac by byly dwa
                Response response = syncClient.SendRequest(request20);
                // Move
                response = syncClient.SendRequest(new CopyFileRequest(@"C: \Users\marlena.knitter\Desktop\RoboBox\TESTPROGRAM\COPY\test" , @"KRC:/R1/PROGRAM/" , true));
                Console.WriteLine("Command: {0}, Success: {1}, ErrorCode: {2}", response.Type, response.Success, response.ErrorCode);
                Console.ReadLine();
            }


        }
      
    }
}




