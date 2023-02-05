using BackendDevTest.Helper;
using BackendDevTest.Model;
using Newtonsoft.Json;

namespace BackendDevTest.Service
{
    public class EtherscanService : IEtherscanService
    {
        private readonly HttpClient _client;

        public EtherscanService(HttpClient client)
        {
            _client = client;
        }
        public async Task<BlocksModel> GetBlockByNumber(string blockNumber)
        {
            BlocksModel model = new BlocksModel();
            HttpResponseMessage response = await _client.GetAsync($"/api?module=proxy&action=eth_getBlockByNumber&tag=0x{blockNumber}&boolean=true&apikey={CommonHelper.APIKey}");
            if (response.IsSuccessStatusCode)
            {
                string result = await response.Content.ReadAsStringAsync();
                model = JsonConvert.DeserializeObject<BlocksModel>(result);
            }

            return model;
        }

        public async Task<TransactionBlockResutModel> GetBlockTransactionCountByNumber(string blockNumber)
        {
            TransactionBlockResutModel model = new TransactionBlockResutModel();
            HttpResponseMessage response = await _client.GetAsync($"/api?module=proxy&action=eth_getBlockTransactionCountByNumber&tag=0x{blockNumber}&apikey={CommonHelper.APIKey}");
            if (response.IsSuccessStatusCode)
            {
                string result = await response.Content.ReadAsStringAsync();
                model = JsonConvert.DeserializeObject<TransactionBlockResutModel>(result);
            }

            return model;
        }

        public async Task<TransactionsModel> GetTransactionByBlockNumberAndIndex(string blockNumber, string transactionIndex)
        {
            TransactionsModel model = new TransactionsModel();
            HttpResponseMessage response = await _client.GetAsync($"/api?module=proxy&action=eth_getTransactionByBlockNumberAndIndex&tag=0x{blockNumber}&index={transactionIndex}&apikey={CommonHelper.APIKey}");
            if (response.IsSuccessStatusCode)
            {
                string result = await response.Content.ReadAsStringAsync();
                model = JsonConvert.DeserializeObject<TransactionsModel>(result);
            }

            return model;
        }

        public string InsertBlocks(BlocksModel block)
        {
            string query = $"INSERT INTO blocks (blockNumber, hash, parentHash, miner, blockReward, gasLimit, gasUsed)" +
                $" VALUES ('{CommonHelper.ConvertNumberToHEX(block.Result.Number)}','{CommonHelper.RemoveHEXString(block.Result.Hash)}'," +
                $"'{CommonHelper.RemoveHEXString(block.Result.ParentHash)}'," +
                $"'{CommonHelper.RemoveHEXString(block.Result.Miner)}', '{CommonHelper.ConvertHEXStringToDecimal(block.Result.Size)}', " +
                $"'{CommonHelper.ConvertHEXStringToDecimal(block.Result.GasLimit)}', {CommonHelper.ConvertHEXStringToDecimal(block.Result.GasUsed)})";

            return query;
        }

        public string InsertTransactions(TransactionInfoModel transaction)
        {
            int blockNumber = Convert.ToInt32(CommonHelper.ConvertNumberToHEX(transaction.BlockNumber));
            string query = $" ((SELECT blockID from blocks WHERE blockNumber = {blockNumber})," +
                $"'{CommonHelper.RemoveHEXString(transaction.Hash)}','{CommonHelper.RemoveHEXString(transaction.From)}'," +
                $"'{CommonHelper.RemoveHEXString(transaction.To)}', '{CommonHelper.RemoveHEXString(transaction.Value)}', '{CommonHelper.ConvertHEXStringToDecimal(transaction.Gas)}'," +
                $" '{CommonHelper.ConvertHEXStringToDecimal(transaction.GasPrice)}','{CommonHelper.ConvertNumberToHEX(transaction.TransactionIndex)}')";

            return query;
        }
    }
}
