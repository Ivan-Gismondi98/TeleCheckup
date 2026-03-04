using Microsoft.Maui.Hosting;

namespace TeleCheckup.Extensions
{
    public static class FirebaseMauiExtensions
    {
        public static MauiAppBuilder UseFirebaseApp(this MauiAppBuilder builder)
        {
            // No-op stub for platforms where Firebase native initialization isn't available in this environment.
            return builder;
        }
    }
}
