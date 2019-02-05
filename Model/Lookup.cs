using System;
using System.Runtime.Serialization;

namespace Target365.Sdk
{
	/// <summary>
	/// Lookup result object.
	/// </summary>
	[DataContract(Namespace = "http://schemas.target365.io/core")]
	public class Lookup
	{
		/// <summary>
		/// Created.
		/// </summary>
		[DataMember]
		public DateTimeOffset Created { get; set; }

		/// <summary>
		/// Msisdn.
		/// </summary>
		[DataMember]
		public string Msisdn { get; set; }

		/// <summary>
		/// Landline phone number.
		/// </summary>
		[DataMember]
		public string Landline { get; set; }

		/// <summary>
		/// First name.
		/// </summary>
		[DataMember]
		public string FirstName { get; set; }

		/// <summary>
		/// Middle name.
		/// </summary>
		[DataMember]
		public string MiddleName { get; set; }

		/// <summary>
		/// Last name.
		/// </summary>
		[DataMember]
		public string LastName { get; set; }

		/// <summary>
		/// Company name.
		/// </summary>
		[DataMember]
		public string CompanyName { get; set; }

		/// <summary>
		/// Company organization number.
		/// </summary>
		[DataMember]
		public string CompanyOrgNo { get; set; }

		/// <summary>
		/// Street name.
		/// </summary>
		[DataMember]
		public string StreetName { get; set; }

		/// <summary>
		/// Street number.
		/// </summary>
		[DataMember]
		public string StreetNumber { get; set; }

		/// <summary>
		/// Street letter.
		/// </summary>
		[DataMember]
		public string StreetLetter { get; set; }

		/// <summary>
		/// Zip code.
		/// </summary>
		[DataMember]
		public string ZipCode { get; set; }

		/// <summary>
		/// City.
		/// </summary>
		[DataMember]
		public string City { get; set; }

		/// <summary>
		/// Gender, 'M' for male, 'F' for female and 'U' for unknown.
		/// </summary>
		[DataMember]
		public string Gender { get; set; }

		/// <summary>
		/// Date of birth, in format 'yyyy-dd-MM'.
		/// </summary>
		[DataMember]
		public string DateOfBirth { get; set; }

		/// <summary>
		/// Age.
		/// </summary>
		[DataMember]
		public int? Age { get; set; }

		/// <summary>
		/// Deceased date, in format 'yyyy-dd-MM'.
		/// </summary>
		[DataMember]
		public string DeceasedDate { get; set; }
	}
}
