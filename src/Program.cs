using System;
using System.Threading.Tasks;
using Refit;
using SlackPublications.SlackApi;

namespace SlackPublications
{
    /// <summary>
    /// Main class. Takes input and produces output. :-)
    /// </summary>
    class Program
    {
        /// <summary>
        /// Runs the program. Provide your Slack OAuth token either as a parameter, or when prompted.
        /// </summary>
        public static async Task Main(string[] args)
        {
            string token = GetSlackToken(args);

            if (string.IsNullOrWhiteSpace(token))
            {
                Console.WriteLine("Invalid Slack token.");
                return;
            }

            ISlackApi slackClient = new SlackClient(token);
            PublicationsGenerator generator = new PublicationsGenerator(slackClient);

            string publications = await generator.GeneratePublicaties();

            System.Console.WriteLine(publications);
        }

        /// <summary>
        /// Gets the Slack token from either the command line parameters, or asks the user for it.
        /// </summary>
        private static string GetSlackToken(string[] args)
        {
            string token;

            if (args.Length != 1)
            {
                Console.Write("Enter your Slack OAuth token (or run this program with the token as the first parameter): ");
                token = Console.ReadLine();
            }
            else
            {
                token = args[0];
            }

            return token;
        }
    }
}
