namespace AvMock.Enums
{
    [Flags]
    public enum SecurityThreatNames
    {
        None = 0,

        Virus = 1 << 0,
        Worm = 1 << 1,
        Trojan = 1 << 2,
        Ransomware = 1 << 3,
        Spyware = 1 << 4,

        Adware = 1 << 5,
        PotentiallyUnwantedPrograms = 1 << 6,

        Phishing = 1 << 7,
        ManInTheMiddle = 1 << 8,
        DDoS = 1 << 9,

        Rootkit = 1 << 10,
        Keylogger = 1 << 11,
        Backdoor = 1 << 12,

        Malware = Virus | Worm | Trojan | Ransomware | Spyware,
        NetworkThreats = Phishing | ManInTheMiddle | DDoS,
        OtherThreats = Rootkit | Keylogger | Backdoor,

        All = Malware | Adware | PotentiallyUnwantedPrograms | NetworkThreats | OtherThreats
    }
}