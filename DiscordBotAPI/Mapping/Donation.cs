using DiscordBotAPI.Mapping;

namespace TNSApi.Mapping
{
    public enum DonationResult
    {
        DonationSuccesful,
        DonatorNoMoney,
        DonatorDoesntExist,
        ReceiverDoesntExist,
        UnknownError
    }


    public class Donation
    {
        public User Donator;
        public User Receiver;
        public long Points;
        public DonationResult Result;
    }
}