using DnsClient;
using System.Text.RegularExpressions;

namespace API.Utils.Validators
{
    public static class FormatValidator
    {
        public static bool ValidateE164Format(string phoneNumber)
        {

            var pattern = @"^\+55(11|12|13|14|15|16|17|18|19" +
                             "|21|22|24|27|28|31|32|33|34|35" +
                             "|37|38|41|42|43|44|45|46|47|48" +
                             "|49|51|53|54|55|61|62|63|64|65" +
                             "|66|67|68|69|71|73|74|75|77|79" +
                             "|81|82|83|84|85|86|87|88|89|91" +
                             "|92|93|94|95|96|97|98|99)" +
                             "(9[0-9]{8}|[2-9][0-9]{7})$";

            var regex = new Regex(pattern);
            var validationResult = regex.IsMatch(phoneNumber);

            return validationResult;
        }
        public static bool ValidateMatriculatesFormat(string matriculates)
        {
            var pattern = @"^[0-9]{15}$";

            var regex = new Regex(pattern);
            var validationResult = regex.IsMatch(matriculates);

            return validationResult;
        }
        public static async Task<bool> ValidateDomainFormat(string domain)
        {
            var lookup = new LookupClient();
            var result = await lookup.QueryAsync(domain, QueryType.MX);

            return result.Answers.MxRecords().Any();
        }
    }
}
