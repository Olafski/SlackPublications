using System;
using System.Threading.Tasks;
using Refit;
using SlackPublicaties.SlackClient;

namespace SlackPublicaties
{
    class Program
    {
        public static async Task Main(string[] args)
        {
            string token;

            if (args.Length != 1)
            {
                Console.Write("Voer je Slack OAuth token in (of voer dit programma uit met het token als eerste parameter): ");
                token = Console.ReadLine();
            }
            else
            {
                token = args[0];
            }

            if (string.IsNullOrWhiteSpace(token))
            {
                Console.WriteLine("Ongeldig Slack token.");
                return;
            }

            PublicatiesGenerator generator = new PublicatiesGenerator(token);

            string publicaties = await generator.GenereerPublicaties();

            System.Console.WriteLine(publicaties);
        }
    }
}
