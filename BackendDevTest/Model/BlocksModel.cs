namespace BackendDevTest.Model
{
    public class BlocksModel
    {
        public BlocksInfoModel Result { get; set; }
    }

    public class BlocksInfoModel
    {
        public string Id { get; set; }
        public string Number { get; set; }
        public string Hash { get; set; }
        public string ParentHash { get; set; }
        public string Miner { get; set; }
        public string Size { get; set; }
        public string GasLimit { get; set; }
        public string GasUsed { get; set; }
        public List<TransactionInfoModel> Transactions { get; set; }
    }
}
