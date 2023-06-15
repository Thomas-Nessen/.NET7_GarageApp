using GarageApp.Models;
using DBHelper;
//using GarageApp.Factories;
//using static IronPython.Modules._ast;
//using static IronPython.Modules.PythonStruct;
//using static IronPython.Runtime.Profiler;
//using static System.Net.Mime.MediaTypeNames;
using System;
using System.Data.Common;

namespace GarageApp.DAL
{
    public class Work : IWork
    {
        private readonly string conStr = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=GarageDB;Integrated Security=True";
        
        public async Task<int> AddParkedCarAsync(ParkedCarData data)
        {
            //Function to add a car to the DB
            DBCommander cmd = new(conStr, true, "stp_AddCar");
            cmd.AddParam("@Name", data.Name);
            cmd.AddParam("@CarID", data.CarID);
            cmd.AddParam("@ParkingSpot", data.ParkingSpot);
            return await cmd.ExecuteStpNonQueryAsync();
        }
        //public async Task<ParkedCarData> GetParkedCarByIdAsync(int id)
        //{
        //    string sql = "select p.[Lastname], p.[BusinessEntityID], p.[FirstName] from [Person].[Person] p where [BusinessEntityID] = " + id.ToString();
        //    DBCommander cmd = new(conStr, false, sql);

        //    ParkedCarData? sd = await cmd.RetrieveFirstFromDataReaderAsync(SimpleFactory.CreateSomeData);

        //    if (sd != null) return sd;
        //    return new ParkedCarData { BusinessEntityId = 0, FirstName = "", LastName = "" };
        //}

        //    public async Task<ParkedCarData> RemoveParkedCarByIdAsync(int id)
        //    {
        //        string sql = "select p.[Lastname], p.[BusinessEntityID], p.[FirstName] from [Person].[Person] p where [BusinessEntityID] = " + id.ToString();
        //        DBCommander cmd = new(conStr, false, sql);

        //        ParkedCarData? sd = await cmd.RetrieveFirstFromDataReaderAsync(SimpleFactory.CreateSomeData);

        //        if (sd != null) return sd;
        //        return new SomeData { BusinessEntityId = 0, FirstName = "", LastName = "" };
        //    }

        //    public async Task<List<ParkedCarData>> GetAllDataAsync(int bid)
        //    {
        //        DBCommander cmd = new(conStr, true, "uspGetEmployeeManagers");
        //        cmd.AddParam("@BusinessEntityID", bid);

        //        return await cmd.RetrieveListFromDataReaderAsync(SimpleFactory.CreateSomeData);
        //    }




        //    public async Task<List<ParkedCarData>> GetListAsync()
        //    {
        //        // Function to get List of data from the DB
        //        DBCommander cmd = new(conStr,
        //            false,
        //            "SELECT c.[Name] FROM [dbo].[TEST] c ORDER BY c.[Name]");

        //        return await cmd.RetrieveListFromDataReaderAsync(SimpleFactory.CreateSomeData);
        //    }

    }
}
