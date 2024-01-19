using Home.Extractor.Controllers;
using Home.Extractor.Entities;
using System;
using System.Text.RegularExpressions;

namespace Home.Extractor.Parsers {
    public class CapraboTicketParser : ITicketParser {

        Regex dateRegex = new Regex("C:.* ([0-9][0-9])\\/([0-9][0-9])\\/([0-9][0-9][0-9][0-9]) ([0-9][0-9]):([0-9][0-9]) .*", RegexOptions.Compiled | RegexOptions.Singleline);
        //Regex decimalRegex = new Regex("(\\d*,[0-9][0-9])", RegexOptions.Compiled | RegexOptions.Singleline);
        Regex decimalRegex = new Regex("(\\d*,\\d*)", RegexOptions.Compiled | RegexOptions.Singleline);

        private enum ParserState {
            ReadingDate,
            ReadingItems,
            ReadingTotal,
            ReadingPayment
        }

        private ParserState state = ParserState.ReadingDate;

        public Ticket Parse(string ticketData) {
            var ticket = new Ticket();

            ticket.Shop = "Caprabo";

            var lines = ticketData.Split("\n");

            for (int i = 0; i < lines.Length; i++) {

                var line = lines[i];

                if (line == string.Empty) continue;

                switch (state) {

                    case ParserState.ReadingDate:
                        var match = dateRegex.Match(line);
                        if (match.Success) {
                            //TODO force culture
                            var dateString = $"{match.Groups[3].Value}-{match.Groups[2].Value}-{match.Groups[1].Value} {match.Groups[4].Value}:{match.Groups[5].Value}:00";
                            var date = DateTime.Parse(dateString);
                            ticket.EmitedAt = date;
                            state = ParserState.ReadingItems;
                        }
                        break;

                    case ParserState.ReadingItems:
                        if (line.StartsWith("Uni", StringComparison.CurrentCultureIgnoreCase)) {
                            i++;
                            ticket.Items = ReadItems(lines, ref i);
                            state = ParserState.ReadingTotal;
                        }
                        break;
                    case ParserState.ReadingTotal:
                        if (line.StartsWith("TOTAL A PAGAR", StringComparison.CurrentCultureIgnoreCase)) {

                            var totalMatch = decimalRegex.Matches(line);

                            var dd = totalMatch.Where(x => x.Success).SelectMany(x => x.Captures).ToArray();

                            if (!dd.Any()) {
                                throw new Exception("TOTAL A PAGAR regex decimals error:" + line);
                            }
                            ticket.TotalPaid = float.Parse(dd[0].Value.Replace(",", "."));
                            state = ParserState.ReadingPayment;
                        }
                        break;
                    case ParserState.ReadingPayment:
                        if (line.StartsWith("N.T", StringComparison.CurrentCultureIgnoreCase)) {

                        }
                        break;
                }
            }

            ticket.Total = ticket.Items.Sum(x => x.TotalPrice);

            return ticket;
        }

        private TicketItem[] ReadItems(string[] lines, ref int i) {
            var items = new List<TicketItem>();

            for (; i < lines.Length; i++) {

                var line = lines[i];
                if (line == string.Empty) continue;

                if (line.StartsWith("TOTALS")) {
                    i--;
                    return items.ToArray();
                }

                var item = ParseItem(line);
                items.Add(item);
            }

            throw new Exception("Read items overrun");
        }
        //LEVITÉ LLIMONA 1, 1,75

        private TicketItem ParseItem(string line) {
            var item = new TicketItem();

            item.Quantity = 1;

            line = line.Replace(",/", ",");
            var match = decimalRegex.Matches(line);

            var dd = match.Where(x => x.Success).SelectMany(x => x.Captures).ToArray();

            if (!dd.Any()) {
                throw new Exception("regex decimals");
            }

            var decimals = dd;

            var commaCount = line.Count(x => x == ',');

            int nameStartIndex = 0;
            int nameEndIndex = 1;

            if (decimals.Length == 1) {

                item.TotalPrice = float.Parse(decimals[0].Value.Replace(",", ".")); //PriceFromEnd(line, out int firstCharIndex);
                nameEndIndex = decimals[0].Index - 1;
            }
            else if (decimals.Length == 2) {
                var firstSpace = line.IndexOf(' ');
                var quantityString = line[..firstSpace];

                var lastComma = line.LastIndexOf(',');

                var betweend = line.PreviousCharIndex(lastComma - 1, ',');

                if (int.TryParse(quantityString, out var result)) {
                    nameStartIndex = firstSpace + 1;
                    item.Quantity = result;
                    item.TotalPrice = float.Parse(decimals[1].Value.Replace(",", "."));
                    nameEndIndex = line.PreviousCharIndex(decimals[0].Index, ' ');
                }
                else {

                    var startPriceIndex = line.PreviousCharIndex(betweend, ' ') + 1;

                    betweend += 3;
                    nameEndIndex = startPriceIndex - 1;
                    var priceString = line[startPriceIndex..betweend].Replace(",", ".");

                    item.TotalPrice = float.Parse(priceString);
                }

            }
            else if (decimals.Length == 3) {
                throw new NotImplementedException("3 commas");
                //var firstSpace = line.IndexOf(' ');
                //var quantityString = line[..firstSpace];
                //item.Quantity = int.Parse(quantityString);
            }
            else {
                throw new Exception("commas error");
            }


            item.Description = line[nameStartIndex..nameEndIndex];

            return item;
        }


        private TicketItem ParseItem2(string line) {
            var item = new TicketItem();

            item.Quantity = 1;

            var commaCount = line.Count(x => x == ',');

            int nameStartIndex = 0;
            int nameEndIndex = 1;

            if (commaCount == 1) {
                item.TotalPrice = PriceFromEnd(line, out int firstCharIndex);
                nameEndIndex = firstCharIndex;
            }
            else if (commaCount == 2) {
                var firstSpace = line.IndexOf(' ');
                var quantityString = line[..firstSpace];

                var lastComma = line.LastIndexOf(',');

                var betweend = line.PreviousCharIndex(lastComma - 1, ',');

                if (int.TryParse(quantityString, out var result)) {
                    nameStartIndex = firstSpace + 1;
                    item.Quantity = result;
                    item.TotalPrice = PriceFromEnd(line, out int firstCharIndex);
                    nameEndIndex = line.PreviousCharIndex(betweend, ' ');
                }
                else {

                    var startPriceIndex = line.PreviousCharIndex(betweend, ' ') + 1;

                    betweend += 3;
                    nameEndIndex = startPriceIndex - 1;
                    var priceString = line[startPriceIndex..betweend].Replace(",", ".");

                    item.TotalPrice = float.Parse(priceString);
                }

            }
            else if (commaCount == 3) {
                throw new NotImplementedException("3 commas");
                //var firstSpace = line.IndexOf(' ');
                //var quantityString = line[..firstSpace];
                //item.Quantity = int.Parse(quantityString);
            }
            else {
                throw new Exception("commas error");
            }


            item.Description = line[nameStartIndex..nameEndIndex];

            return item;
        }

        private float PriceFromEnd(string line, out int firstCharIndex) {
            var lastSpace = line.LastIndexOf(' ');

            firstCharIndex = lastSpace;

            lastSpace++;

            var priceString = line[lastSpace..].Replace(",", ".");

            return float.Parse(priceString);
        }
    }
}
