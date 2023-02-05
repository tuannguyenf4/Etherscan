namespace BackendDevTest.Model
{
    public class TransactionsModel
    {
        public TransactionInfoModel Result { get; set; }
    }

    public class TransactionInfoModel
    {
        public string Id { get; set; }
        public string BlockNumber { get; set; }
        public string Hash { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public string Value { get; set; }
        public string Gas { get; set; }
        public string GasPrice { get; set; }
        public string TransactionIndex { get; set; }
    }
}
