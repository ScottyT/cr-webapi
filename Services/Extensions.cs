using System.Diagnostics.CodeAnalysis;

namespace cr_app_webapi.Services
{
    public static class ValidExtensions
    {
        public static bool IsValid<T>([NotNullWhen(true)]T? doc) =>
            doc is not null;
    }
}