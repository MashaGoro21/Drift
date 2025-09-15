public static class PrefsKeys
{
    public const string SELECTED_CAR = "Selected Car";
    public const string SFX_VOLUME = "SFXVolume";
    public const string MONEY = "Money";

    public static string CarOwned(string carName) => $"{carName} IsOwned";
    public static string CarColor(string carName) => $"{carName} Color";

    public static string CarParameter(string carName, CarParameters parameter)
    {
        switch(parameter)
        {
            case CarParameters.Acceleration:
                return CarAcceleration(carName);
            case CarParameters.Handling:
                return CarHandling(carName);
            case CarParameters.Braking:
                return CarBraking(carName);
        }
        return null;
    }

    public static string CarHandling(string carName) => $"{carName} {CarParameters.Handling}";
    public static string CarAcceleration(string carName) => $"{carName} {CarParameters.Acceleration}";
    public static string CarBraking(string carName) => $"{carName} {CarParameters.Braking}";
}
