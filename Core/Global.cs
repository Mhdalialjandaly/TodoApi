namespace Core
{
    public static class Global
    {      

        public static string DateFormat = "dd/MM/yyyy";
        public static string DeviceName { get; set; } = "Public";

    }

    public static class GlobalKeys
    {
        public const string LoggedUser = nameof(LoggedUser);
    }

}
