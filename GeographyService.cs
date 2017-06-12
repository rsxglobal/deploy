using Sabio.Data;
using Sabio.Web.Domain;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace Sabio.Web.Services
{
    public class GeographyService : BaseService
    {
        public static List<Country> SelectAllCountries()
        {
            List<Country> list = new List<Country>();

            DataProvider.ExecuteCmd(GetConnection, "dbo.Country_SelectAll",
               inputParamMapper: null,
               map: delegate (IDataReader reader, short set)
               {
                   Country country = new Country();
                   int ord = 0; //startingOrdinal

                   country.Id = reader.GetSafeInt32(ord++);
                   country.Name = reader.GetSafeString(ord++);
                   country.Code = reader.GetSafeString(ord++);
                   country.LongCode = reader.GetSafeString(ord++);

                   list.Add(country);

               }
               );

            return list;
        }
        public static List<StateProvince> SelectAllStateProvinces()
        {
            List<StateProvince> list = new List<StateProvince>();

            DataProvider.ExecuteCmd(GetConnection, "dbo.StateProvince_SelectAll",
               inputParamMapper: null,
               map: delegate (IDataReader reader, short set)
               {
                   MapStateProvinces(reader, list);
               }
               );

            return list;
        }

        private static void MapStateProvinces(IDataReader reader, List<StateProvince> list)
        {
            StateProvince stateProvince = new StateProvince();
            int ord = 0; //startingOrdinal

            stateProvince.Id = reader.GetSafeInt32(ord++);
            stateProvince.CountryId = reader.GetSafeInt32(ord++);
            stateProvince.Code = reader.GetSafeString(ord++);
            stateProvince.CountryRegionCode = reader.GetSafeString(ord++);
            stateProvince.IsOnlyStateProvinceFlag = reader.GetSafeBool(ord++);
            stateProvince.Name = reader.GetSafeString(ord++);
            stateProvince.TerritoryId = reader.GetSafeInt32(ord++);

            list.Add(stateProvince);
        }

        public static List<StateProvince> SelectStateProvinceByCountryId(int id)
        {
            List<StateProvince> list = new List<StateProvince>();

            DataProvider.ExecuteCmd(GetConnection, "dbo.StateProvince_SelectByCountryId",
               inputParamMapper: delegate (SqlParameterCollection paramCollection)
               {
                   paramCollection.AddWithValue("@CountryId", id);

               }
               , map: delegate (IDataReader reader, short set)
               {
                   MapStateProvinces(reader, list);
               }
               );

            return list;
        }
    }
}