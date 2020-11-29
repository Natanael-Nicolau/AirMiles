using AirMiles.Master.Helpers.DailyUpdates;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace AirMiles.Master.Controllers.UpdateAPI
{

    [Route("updates/[controller]")]
    public class DailyUpdatesController : Controller
    {
        public DailyUpdatesController()
        {
        }

        public async Task<bool> UpdateTickets()
        {
            try
            {
                HttpClient client = new HttpClient
                {
                    BaseAddress = new Uri("http://10.147.17.1:50000"),
                };

                string url = $"/list";
                HttpResponseMessage response = await client.GetAsync(url);
                string result = await response.Content.ReadAsStringAsync();

                List<TicketUpdateModel> list = JsonConvert.DeserializeObject<List<TicketUpdateModel>>(result);

                //filter tickets with clients

                foreach (var ticket in list)
                {
                    //calculate number of miles to be credited



                    //update client tickets
                    //Update client miles
                }
                return true;

            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}