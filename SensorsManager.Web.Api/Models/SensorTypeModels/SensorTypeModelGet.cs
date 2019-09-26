namespace SensorsManager.Web.Api.Models
{
    public class SensorTypeModelGet
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal MinValue { get; set; }
        public decimal MaxValue { get; set; }
        public int MeasureId { get; set; }
    }
}