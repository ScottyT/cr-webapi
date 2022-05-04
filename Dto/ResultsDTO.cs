using cr_app_webapi.Models;
using MongoDB.Bson;

namespace cr_app_webapi.Dto;

public class ResultsDTO
{
    public List<Report> Reports {get; set;} = new List<Report>();
    public List<Sketch> Sketches {get; set;} = new List<Sketch>();
}