namespace BackendLimpio.DTOs.Common
{
    public class CreatePetDto
    {
        public string Name { get; set; } = "";
        public string Species { get; set; } = "";
        public string Breed { get; set; } = "";
        public int? Age { get; set; }
        public string Sex { get; set; } = "";
        public string LastName { get; set; } = "";
    }
}