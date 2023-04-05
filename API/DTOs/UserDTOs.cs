namespace StudentHub.DTOs
{
	public class UserRegisterDTO
	{
		public string Username { get; set; }
		public string Password { get; set; }
		public UserRoles Role { get; set; }
	}

	public class UserSigninDTO
	{
		public string Username { get; set; }
		public string Password { get; set; }
	}

	public class UserSigninResponseDTO
	{
		public string accessToken { get; set; }
		public UserRoles role { get; set; }
	}
}
