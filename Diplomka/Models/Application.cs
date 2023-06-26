namespace Diplomka.Models
{
    public class Application //заявки
    {
        public int ApplicationID { get; set; }
        public int? FactoryID { get; set; }
        public  Factory Factory { get; set; }
        public int? WarehouseID { get; set; }
        public Warehouse Warehouse { get; set; }

        public int? DepotID { get; set; }
        public Depot Depot { get; set; }

        public int? CarID { get; set; }
        public Car Car { get; set; }

        public int? DriverID { get; set; }
        public Driver Driver { get; set; }
    }
}
