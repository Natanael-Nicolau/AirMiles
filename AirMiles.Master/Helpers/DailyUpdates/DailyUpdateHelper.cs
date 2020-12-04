using AIrMiles.WebApp.Common.Data.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AirMiles.Master.Helpers.DailyUpdates
{
    public class DailyUpdateHelper : IDailyUpdateHelper
    {
        private readonly IAirportRepository _airportRepository;

        public DailyUpdateHelper(IAirportRepository airportRepository)
        {
            _airportRepository = airportRepository;
        }

        public double CalculateRegionModifier(string startRegion, string endRegion, string flightClass)
        {
            double regionModifier;

            if ((startRegion == "Europa" || startRegion == "North Africa") && (endRegion == "Europa" || endRegion == "North Africa"))
            {
                switch (flightClass)
                {
                    case "Discount":
                        regionModifier = .1;
                        break;
                    case "Basic":
                        regionModifier = .4;
                        break;
                    case "Classic":
                        regionModifier = .7;
                        break;
                    case "Plus":
                        regionModifier = 1;
                        break;
                    case "Executive":
                        regionModifier = 1.5;
                        break;
                    case "Top Executive":
                        regionModifier = 2;
                        break;
                    default:
                        regionModifier = .5;
                        break;
                }
            }
            else
            {
                switch (flightClass)
                {
                    case "Discount":
                        regionModifier = .1;
                        break;
                    case "Basic":
                        regionModifier = .5;
                        break;
                    case "Classic":
                        regionModifier = 1;
                        break;
                    case "Plus":
                        regionModifier = 1.5;
                        break;
                    case "Executive":
                        regionModifier = 1.5;
                        break;
                    case "Top Executive":
                        regionModifier = 2;
                        break;
                    default:
                        regionModifier = .5;
                        break;
                }
            }

            return regionModifier;
        }

        public async Task<int> CalculateTicketMilesAsync(TicketUpdateModel ticket)
        {
            var startAirport = await _airportRepository.GetByIataAsync(ticket.StartIATA);
            var endAirport = await _airportRepository.GetByIataAsync(ticket.EndIATA);
            if (startAirport == null || endAirport == null)
            {
                return 0;
            }

            
            return Convert.ToInt32(
                Math.Truncate(
                    distance(
                        Convert.ToDouble(startAirport.Latitude),
                        Convert.ToDouble(startAirport.Longitude),
                        Convert.ToDouble(endAirport.Latitude),
                        Convert.ToDouble(endAirport.Longitude)
                )));
        }


        private double distance(double lat1, double lon1, double lat2, double lon2)
        {
            if ((lat1 == lat2) && (lon1 == lon2))
            {
                return 0;
            }
            else
            {
                double theta = lon1 - lon2;
                double dist = Math.Sin(deg2rad(lat1)) * Math.Sin(deg2rad(lat2)) + Math.Cos(deg2rad(lat1)) * Math.Cos(deg2rad(lat2)) * Math.Cos(deg2rad(theta));
                dist = Math.Acos(dist);
                dist = rad2deg(dist);
                dist = dist * 60 * 1.1515;
                return (dist);
            }
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //::  This function converts decimal degrees to radians             :::
        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        private double deg2rad(double deg)
        {
            return (deg * Math.PI / 180.0);
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //::  This function converts radians to decimal degrees             :::
        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        private double rad2deg(double rad)
        {
            return (rad / Math.PI * 180.0);
        }
    }
}
