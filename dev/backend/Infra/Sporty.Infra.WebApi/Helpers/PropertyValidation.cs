using System;

namespace Sporty.Infra.WebApi.Helpers
{
    public static class PropertyValidation
    {
        public static bool IsValidDateTime(DateTime date) => date != default;
    }
}
