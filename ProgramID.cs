using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using C3SharpInterface;
using C3SharpInterface.Requests;
using C3SharpInterface.Responses;
using Variable;
using RestSharp;
using Microsoft.AspNetCore.Mvc;
using Catalog.Repositiories;
using System.Collections;
using Catalog.Entities;
using Catalog.Dtos;

namespace ProgramNummerCheck


{
    public class ProgramNummer
    {
        static void Main(string[] args)
        {
            SyncClient syncClient = new SyncClient();
            syncClient.ConnectToHost(IPAddress.Parse(args.Length > 0 ? args[0] : "192.168.1.12"), 7000);



            string data = DateTime.Now.ToString("yyyy-MM-dd");


            Variable.Variables ProgramCheck = new Variable.Variables();


            FilePropertiesRequest request26 = new FilePropertiesRequest(@"KRC:\R1\Program\ROWA\"+ ProgramCheck.Name);
            FilePropertiesResponse response26 = (FilePropertiesResponse)syncClient.SendRequest(request26);
            Console.WriteLine("Command: {0}, Success: {1}, ErrorCode: {2}", response26.Type, response26.Success, response26.ErrorCode);

            if (response26.Success)
            {
                Console.WriteLine("    Name: {0}", response26.FileName);
                Console.WriteLine("    Last Write Time: {0}", response26.LastWriteTime);
                bool VersionBool = true;
                string VersDat = Convert.ToString(response26.LastWriteTime);

                if (ProgramCheck.VersionCheck != VersDat)
                {
                    ProgramCheck.VersionOnRobot++;

                    VersDat = ProgramCheck.VersionCheck;
                    if (syncClient.SendRequest(new CreateFileRequest(@"KRC:\R1\PROGRAM\" + ProgramCheck.Name)).Success)
                    {
                        SetFileAttributesRequest request20 = new SetFileAttributesRequest(@"KRC:/R1/PROGRAM/" + ProgramCheck.Name, ItemAttribute.ReadOnly, ItemAttribute.ReadOnly); // nie wiem czy dziala, sprawdzic potem! jak nie to wykobinowac by byly dwa
                        Response response = syncClient.SendRequest(request20);
                        // Move
                        response = syncClient.SendRequest(new CopyFileRequest(@"KRC:\R1\PROGRAM\"+ ProgramCheck.Name, @"MIEJSCE Z DYSKU\"+ ProgramCheck.Name + data, true));
                        Console.WriteLine("Command: {0}, Success: {1}, ErrorCode: {2}", response.Type, response.Success, response.ErrorCode);

                    }
                }




            }

            if (!response26.Success)
            {
                bool VersionBool = false;
                SetFileAttributesRequest request20 = new SetFileAttributesRequest(@"C:\Users\marlena.knitter\Desktop\RoboBox\Programs" + ProgramCheck.Name, ItemAttribute.ReadOnly, ItemAttribute.ReadOnly); // nie wiem czy dziala, sprawdzic potem! jak nie to wykobinowac by byly dwa
                Response response = syncClient.SendRequest(request20);
                // Move
                response = syncClient.SendRequest(new CopyFileRequest(@"C:\Users\marlena.knitter\Desktop\RoboBox\Programs" + ProgramCheck.Name, @"KRC:\R1\PROGRAM\" + ProgramCheck.Name, true));
                Console.WriteLine("Command: {0}, Success: {1}, ErrorCode: {2}", response.Type, response.Success, response.ErrorCode);

            }
        }
    }
}




