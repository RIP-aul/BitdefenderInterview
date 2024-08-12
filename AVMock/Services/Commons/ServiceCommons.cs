using AvMock.Enums;
using Bogus;

namespace AvMock.Services.Commons
{
    public static class ServiceCommons
    {
        public static Faker Faker { get; } = new Faker("en_US");

        public static void GenerateFile(float threatProbability, out bool isThreat, out AntivirusDetectionResult file)
        {
            var path = string.Concat(Faker.Random.Enum<Drives>(), ':', Faker.System.FilePath());

            isThreat = IsThreat(threatProbability);
            file = new AntivirusDetectionResult(
                path,
                isThreat
                    ? Faker.Random.Enum(SecurityThreatNames.None, SecurityThreatNames.All) // remove None and All security threats
                    : SecurityThreatNames.None);
        }

        private static bool IsThreat(float threatProbability)
            => Faker.Random.Bool(threatProbability);

        private enum Drives
        {
            C,
            D,
            E
        }
    }
}
