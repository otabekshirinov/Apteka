using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

namespace Diplomka.Models
{
    public class Grain  //виды лекартсв
    {
        public int GrainID { get; set; }
        public int Identity { get; set; }
        public string Name { get; set; }
        public int Price { get; set; }
        public string Img { get; set; }


        public List<Order> Orders { get; set; }
        public Grain()
        {
            Orders = new List<Order>();
        }

    }
}
