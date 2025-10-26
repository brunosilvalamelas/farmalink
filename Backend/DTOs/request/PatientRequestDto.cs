using System.ComponentModel.DataAnnotations;

namespace Backend.DTOs.request;

public class PatientRequestDto
{
        /// <summary>
        /// Full name of the person.
        /// Maximum length of 20 characters.
        /// </summary>
        [Required]
        [StringLength(20)]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Email address of the person.
        /// Must be a valid email format.
        /// Maximum length of 50 characters.
        /// </summary>
        [Required]
        [EmailAddress]
        [StringLength(50)]
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Phone number of the person.
        /// Must follow the Portuguese phone number format (optional +351 prefix).
        /// </summary>
        [Required]
        [RegularExpression(@"^(?:\+351)? ?[29][0-9]{8}$")]
        public string PhoneNumber { get; set; } = string.Empty;

        /// <summary>
        /// Street address of the person.
        /// Maximum length of 50 characters.
        /// </summary>
        [Required]
        [StringLength(50)]
        public string Address { get; set; } = string.Empty;

        /// <summary>
        /// Postal code of the person's address.
        /// Must follow the Portuguese format (e.g., 1234-567).
        /// Maximum length of 20 characters.
        /// </summary>
        [Required]
        [StringLength(20)]
        [RegularExpression(@"^\d{4}-\d{3}$")]
        public string ZipCode { get; set; } = string.Empty;
}