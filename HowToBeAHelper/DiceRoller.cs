using System;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace HowToBeAHelper
{
    internal static class DiceRoller
    {
        private static readonly RNGCryptoServiceProvider RngCsp = new RNGCryptoServiceProvider();

        internal static Result Roll(string name, int val, int bonus, string user = null)
        {
            if (val > 100)
                val = 100;
            val += bonus;
            int rolled = RollDice(100);
            int diff = Math.Abs(val - rolled);
            bool success = val >= rolled;
            Crit crit = rolled == 1 ? Crit.Positive : rolled == 100 ? Crit.Negative : Crit.None;
            if (crit == Crit.None)
            {
                int positiveCritArea = (int)Math.Round(val * 0.1f);
                if (rolled <= positiveCritArea && success)
                {
                    crit = Crit.Positive;
                }
                else if(!success)
                {
                    int negativeCritArea = 100 - positiveCritArea;
                    if (rolled >= negativeCritArea)
                    {
                        crit = Crit.Negative;
                    }
                }
            }

            return new Result
            {
                Name = name, Crit = crit, Diff = diff, Rolled = rolled, Success = success, User = user
            };
        }

        internal static async Task SyncWithHost(Result result)
        {
            if (Bootstrap.System.IsSessionView)
            {
                await MainForm.Instance.Master.SyncDiceRoll(Bootstrap.System.User, Bootstrap.System.CurrentSession.ID,
                    JsonConvert.SerializeObject(result));
            }
        }

        public class Result
        {
            [JsonProperty("user")]
            public string User { get; set; }

            [JsonProperty("name")]
            public string Name { get; set; }

            [JsonProperty("rolled")]
            public int Rolled { get; set; }

            [JsonProperty("diff")]
            public int Diff { get; set; }

            [JsonProperty("success")]
            public bool Success { get; set; }

            [JsonProperty("crit")]
            public Crit Crit { get; set; }
        }

        public enum Crit
        {
            None, Negative, Positive
        }

        private static byte RollDice(byte numberSides)
        {
            if (numberSides <= 0)
                throw new ArgumentOutOfRangeException("numberSides");
            byte[] randomNumber = new byte[1];
            do
            {
                RngCsp.GetBytes(randomNumber);
            }
            while (!IsFairRoll(randomNumber[0], numberSides));
            return (byte)(randomNumber[0] % numberSides + 1);
        }

        private static bool IsFairRoll(byte roll, byte numSides)
        {
            int fullSetsOfValues = byte.MaxValue / numSides;
            return roll < numSides * fullSetsOfValues;
        }
    }
}
