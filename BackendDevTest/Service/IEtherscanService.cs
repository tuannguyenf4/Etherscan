using BackendDevTest.Model;

namespace BackendDevTest.Service
{
    public interface IEtherscanService
    {
        public Task<BlocksModel> GetBlockByNumber(string blockNumber);
        public Task<TransactionBlockResutModel> GetBlockTransactionCountByNumber(string blockNumber);
        public Task<TransactionsModel> GetTransactionByBlockNumberAndIndex(string blockNumber, string transactionIndex);
        public string InsertBlocks(BlocksModel block);
        public string InsertTransactions(TransactionInfoModel transaction);
    }
}
