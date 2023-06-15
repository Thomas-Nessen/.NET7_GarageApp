using GarageApp.Models;

namespace GarageApp.DAL
{
    public interface IWork
    {
        // This is the interface to our CRUD operations
        Task<int> AddParkedCarAsync(ParkedCarData data);                      //Add car
        //Task<List<ParkedCarData>> GetParkedCarByIdAsync(int carid);      //Get Car data
        //Task<List<ParkedCarData>> RemoveParkedCarByIdAsync(int carid);   //Remove Car data
        //Task<string> UpdateParkedCarAsync(ParkedCarData data);                //Update 
        //Task<List<ParkedCarData>> GetListOfCarsAsync();                  //Get a list of all cars
    }
}
