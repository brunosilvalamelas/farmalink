using System.ComponentModel.DataAnnotations;

namespace Backend.Entities
{
    /// <summary>
    /// Represents a generic person with common identifying and contact information.
    /// Serves as a base class for entities such as patients or doctors.
    /// </summary>
    public abstract class Person
    {
        /// <summary>
        /// Unique identifier for the person.
        /// Primary key.
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Full name of the person.
        /// Maximum length of 20 characters.
        /// </summary>
        [Required(ErrorMessage = "O campo {0} é obrigatório.")]
        [StringLength(20, ErrorMessage = "O campo {0} deve ter no máximo {1} caracteres.")]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Email address of the person.
        /// Must be a valid email format.
        /// Maximum length of 50 characters.
        /// </summary>
        [Required(ErrorMessage = "O campo {0} é obrigatório.")]
        [EmailAddress(ErrorMessage = "O campo {0} não é um endereço de e-mail válido.")]
        [StringLength(50, ErrorMessage = "O campo {0} deve ter no máximo {1} caracteres.")]
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Phone number of the person.
        /// Must follow the Portuguese phone number format (optional +351 prefix).
        /// </summary>
        [Required(ErrorMessage = "O campo {0} é obrigatório.")]
        [RegularExpression(@"^(?:\+351)? ?[29][0-9]{8}$", ErrorMessage = "O campo {0} não é um número de telemóvel português válido.")]
        public string PhoneNumber { get; set; } = string.Empty;

        /// <summary>
        /// Street address of the person.
        /// Maximum length of 50 characters.
        /// </summary>
        [Required(ErrorMessage = "O campo {0} é obrigatório.")]
        [StringLength(50, ErrorMessage = "O campo {0} deve ter no máximo {1} caracteres.")]
        public string Address { get; set; } = string.Empty;

        /// <summary>
        /// Postal code of the person's address.
        /// Must follow the Portuguese format (e.g., 1234-567).
        /// Maximum length of 20 characters.
        /// </summary>
        [Required(ErrorMessage = "O campo {0} é obrigatório.")]
        [StringLength(20, ErrorMessage = "O campo {0} deve no máximo {1} caracteres.")]
        [RegularExpression(@"^\d{4}-\d{3}$", ErrorMessage = "O campo {0} não é um código postal português válido .")]
        public string ZipCode { get; set; } = string.Empty;
    }
}
