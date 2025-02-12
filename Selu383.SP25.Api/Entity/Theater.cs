using Selu383.SP25.Api.Data.Base;
using System.ComponentModel.DataAnnotations;

namespace Selu383.SP25.Api.Entity
{
    public class Theater : IEntityBase
    {
        public int Id { get; set; }

        [Required, MaxLength(120)]
        public string Name { get; set; } = string.Empty;

        [Required]
        public string Address { get; set; } = string.Empty;

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "SeatCount must be at least 1.")]
        public int SeatCount
        {
            get; set;
        }
    }
}