using Microsoft.AspNetCore.Mvc;
using Catalog.Repositiories;
using System.Collections;
using Catalog.Entities;
using Catalog.Dtos;
using C3SharpInterface;
using C3SharpInterface.Requests;
using C3SharpInterface.Responses;
using System.Net;

namespace Catalog.Controllers

{
 
 // GET / items
 [ApiController]
 [Route("v2/entities")]
 public class ItemsController : ControllerBase

 {

    private readonly IItemsRepository repository;
    public ItemsController(IItemsRepository repository)
    {
        this.repository = repository;

    }


        // Max this is the key hier 
        //Here is the at the moment main
    //GET /items
    [HttpGet]
    public IEnumerable<ItemDto> GetItems()

    {
        var items = repository.GetItems().Select(item => item.AsDto());
        return items; 
    }

        //GET /items/id
        [HttpGet("test")] // du musst das entityid momentan  wechseln zum pragram name - ich habe test gemacht weil es soll program wie das sein da // this is entity id, at the moment ist a test for a testing purpose only, but this we should get from server
        public ActionResult<ItemDto> GetItem(Guid id)
        {
           var item = repository.GetItem(id);
            
                 if (item is null)
                   
                {
            return NotFound();
                 }

            // das alles was ist hier soll in ansere Klasse sein, aber wir testen das momentan so // it should be in separate class, kann you maybe help me with this?

                 // hier ist verbindung zum robot, in falls robot ip ist anders du muss das hier wechseln. wichtig ist, das am robot C3bridge schon eingeschaltet ist! // here ist connection to robot with ip
            SyncClient syncClient = new SyncClient();
            syncClient.ConnectToHost(IPAddress.Parse("192.168.1.12"), 7000);

            // hier wir bekommen global Variables 
            Variable.Variables ProgramCheck = new Variable.Variables();

            //wir bekommen data für spatere program schicken 
            string data = DateTime.Now.ToString("yyyy-MM-dd");
                       
            // hier wir bekommen program name 
            ProgramCheck.Name = item.Name;

            // prüfen ob program exiestiert  // here is a check, if program exsist
            FilePropertiesRequest request26 = new FilePropertiesRequest(@"KRC:\R1\Program\ROWA\" + ProgramCheck.Name);
            FilePropertiesResponse response26 = (FilePropertiesResponse)syncClient.SendRequest(request26);
            //  das console hier - wie alle andere ist hier nur für testen, weil hier wir sehen status // console will be later thown away, now is only for test, if data goes
            Console.WriteLine("Command: {0}, Success: {1}, ErrorCode: {2}", response26.Type, response26.Success, response26.ErrorCode);

            // in falls program steht am roboter :
            if (response26.Success)
            {
                Console.WriteLine("    Name: {0}", response26.FileName);
                Console.WriteLine("    Last Write Time: {0}", response26.LastWriteTime);
                // 
                bool VersionBool = true;
                string VersDat = Convert.ToString(response26.LastWriteTime);
                // We should check the program version and if date change is different than saved we should raise the version nummer, it should be for every program the different variable but this i also dont know, maybe would be better to use a DB? help
                // in falls, letzten änderungsdatum ist anders, wir sollen version nummer wechseln und neu Programm schicken  am Server, hier ich habe problemm mit variable benenung, ich muss das irgendwie noch durchdenken
                if (ProgramCheck.VersionCheck != VersDat)
                {
                    ProgramCheck.VersionOnRobot++;

                    // this is in case, thec change date is different, we should sent a new version on server -  it should work 
                    VersDat = ProgramCheck.VersionCheck;
                    if (syncClient.SendRequest(new CreateFileRequest(@"KRC:\R1\PROGRAM\" + ProgramCheck.Name)).Success)
                    {
                        SetFileAttributesRequest request20 = new SetFileAttributesRequest(@"KRC:/R1/PROGRAM/" + ProgramCheck.Name, ItemAttribute.ReadOnly, ItemAttribute.ReadOnly); // hier wir mussen prüfen ob wir bekommen dat + src oder nur eine, ich wollte das gestern Testen (30.05)
                        Response response = syncClient.SendRequest(request20);
                        // Move
                        response = syncClient.SendRequest(new CopyFileRequest(@"KRC:\R1\PROGRAM\" + ProgramCheck.Name, @"PLATZ_ON_SERVER_FUER_DAS_FILE\" + ProgramCheck.Name + data, true));
                        Console.WriteLine("Command: {0}, Success: {1}, ErrorCode: {2}", response.Type, response.Success, response.ErrorCode);


                    }
                }

            }

             // here is what should happen, if robot doesnt have the program - it should get it from server 
            // in falls, wir haben kein version on robot, wir sollen das von server bekommen
            if (!response26.Success)
            {
                bool VersionBool = false;
                SetFileAttributesRequest request20 = new SetFileAttributesRequest(@"C:\Users\marlena.knitter\Desktop\RoboBox\Programs" + ProgramCheck.Name, ItemAttribute.ReadOnly, ItemAttribute.ReadOnly); // nie wiem czy dziala, sprawdzic potem! jak nie to wykobinowac by byly dwa
                Response response = syncClient.SendRequest(request20);
                // Move
                response = syncClient.SendRequest(new CopyFileRequest(@"C:\Users\marlena.knitter\Desktop\RoboBox\Programs" + ProgramCheck.Name, @"KRC:\R1\PROGRAM\" + ProgramCheck.Name, true));
                Console.WriteLine("Command: {0}, Success: {1}, ErrorCode: {2}", response.Type, response.Success, response.ErrorCode);


            }




            return null;

        }


        // we normally should not be this one, who post, we should only put i left this at the moment in case we need 

   //         //POST / items
    //        [HttpPost]
     //       public ActionResult<ItemDto> CreateItem(CreateItemDto itemDto)
   // {
   //     Item item = new()
    //    {
    //        Id = Guid.NewGuid(),
    //       Name = itemDto.Name,
    //        versionOnRobot = itemDto.versionOnRobot, 
    //       
    //
    //    };
    //
    //    repository.CreateItem(item);
    //
    //    return CreatedAtAction(nameof(GetItem), new{ id = item.Id}, item.AsDto());
    //
    //
    //}

    //PUT /items/

    [HttpPut("{entityId}")]
    public ActionResult UpdateItem(Guid id, UpdateItemDto itemDto)
    {
        var existingItem = repository.GetItem(id);

        if (existingItem is null)
        {
            return NotFound();
        }
        // here i shoud post data from up, but this variables are not working -,-
        Item updatedItem = existingItem with
        {
            Name = itemDto.Name,
            versionOnRobot = itemDto.versionOnRobot

        };

        repository.UpdateItem(updatedItem); 

        return NoContent();
       

    }


        // Delte we also not need, is only in case 
    // Delete / items

    //[HttpDelete("{entityId}")]

    //public ActionResult DeleteItem(Guid id)

    //{
    //    var existingItem = repository.GetItem(id);
    //    if (existingItem is null)
    //    {
    //        return NotFound();
        
    //    }

    //    repository.DeleteItem(id);
    //    return NoContent();

    //}
 }
 

}