namespace RpdPlayerApp.Managers;

public static class DeviceIdManager
{
    private const string DEVICE_ID_KEY = "DEVICE_UNIQUE_ID";

    public static async Task<string> GetDeviceIdAsync()
    {
        // Try SecureStorage first
        try
        {
            var id = await SecureStorage.GetAsync(DEVICE_ID_KEY);
            if (!string.IsNullOrWhiteSpace(id)) { return id; }

            // Generate and save a new one
            id = Guid.NewGuid().ToString();
            await SecureStorage.SetAsync(DEVICE_ID_KEY, id);
            return id;
        }
        catch (Exception)
        {
            // If SecureStorage fails (e.g. no device lock), fall back to Preferences
            if (Preferences.ContainsKey(DEVICE_ID_KEY)) { return Preferences.Get(DEVICE_ID_KEY, string.Empty); }

            var id = Guid.NewGuid().ToString();
            Preferences.Set(DEVICE_ID_KEY, id);
            return id;
        }
    }
}
