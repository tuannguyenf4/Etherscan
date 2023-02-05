using System.Text;
using BackendDevTest.Helper;
using BackendDevTest.Model;
using BackendDevTest.Service;
using Microsoft.Extensions.Logging;
using MySql.Data.MySqlClient;

namespace BackendDevTest.BusinessLogic
{
    public class ProcessToIndexBlockService
    {
        private readonly IEtherscanService _etherscanService;
        private readonly ILogger _logger;

        public ProcessToIndexBlockService(IEtherscanService etherscanService, ILogger<ProcessToIndexBlockService> logger)
        {
            _etherscanService = etherscanService;
            _logger = logger;
        }

        public async Task ProcessToIndexBlock()
        {
            // Define the connection abd connect to database
            using (MySqlConnection conn = new MySqlConnection(CommonHelper.ConnectionString))
            {
                // Open the connection
                _logger.LogInformation($"[{CommonHelper.FormatDateTimeToLongString()}]:Connecting to MySQL...");

                conn.Open();

                // Create the transaction
                MySqlTransaction transactionMySQL = conn.BeginTransaction();

                int blockFrom = Convert.ToInt32(CommonHelper.BlockFrom);
                int blockTo = Convert.ToInt32(CommonHelper.BlockTo);

                try
                {
                    // Require to start from Block #12100001 to Block #12100500. 
                    _logger.LogInformation($"[{CommonHelper.FormatDateTimeToLongString()}]: Process to index blocks from the block [{blockFrom}] to [{blockTo}]");
                    for (int blockNo = blockFrom; blockNo <= blockTo; blockNo++)
                    {
                        _logger.LogInformation($"[{CommonHelper.FormatDateTimeToLongString()}]: Process for block Number [{blockNo}]");

                        // 1. Get desired block by blockNumber from Etherscan
                        string blockHex = blockNo.ToString("X");
                        BlocksModel blockModel = await _etherscanService.GetBlockByNumber(blockHex);

                        // If bock is not found
                        if (blockModel.Result == null)
                        {
                            _logger.LogInformation($"There is no data for the block number [{blockNo}]");
                            continue;
                        }

                        // 2.1 Insert the block record into the database
                        string query = _etherscanService.InsertBlocks(blockModel);
                        MySqlCommand cmd = new MySqlCommand(query, conn);
                        cmd.ExecuteNonQuery();

                        // 2.2 Get the count of transaction by block Number from Etherscan
                        TransactionBlockResutModel transactionBlockResutModel = await _etherscanService.GetBlockTransactionCountByNumber(blockHex);
                        string value = transactionBlockResutModel.Result;
                        if (!string.IsNullOrEmpty(value) && transactionBlockResutModel.Result.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
                        {
                            value = value.Substring(2);
                        }

                        // 3.1 Check if the count of transaction is <> 0
                        int noTransactions = int.Parse(value, System.Globalization.NumberStyles.HexNumber);
                        if (noTransactions <= 0)
                        {
                            _logger.LogInformation($"There is no transaction with block number [{blockNo}]");
                            continue;
                        }

                        if (!blockModel.Result.Transactions.Any())
                        {
                            _logger.LogInformation($"There is no transaction with block number [{blockNo}]");
                            continue;
                        }

                        // Get the transactions by tag and transaction index from Etherscan and build the query to insert the transaction into database
                        string commandText = await BuildCommandQueryForTransactions(blockHex, query, blockModel.Result.Transactions);
                        cmd = new MySqlCommand(commandText, conn);
                        cmd.ExecuteNonQuery();

                        _logger.LogInformation($"[{CommonHelper.FormatDateTimeToLongString()}]: Complete to process for the block number [{blockNo}]");
                    }

                    // Commit the transaction
                    _logger.LogInformation($"[{CommonHelper.FormatDateTimeToLongString()}]: Complete to index blocks from the block [{blockFrom}] to [{blockTo}]");
                    transactionMySQL.Commit();
                }
                catch (Exception err)
                {
                    // Incase of there is error and rollback the transaction
                    _logger.LogInformation($"[{CommonHelper.FormatDateTimeToLongString()}]: There is error with error message '{err.Message}'");
                    transactionMySQL.Rollback();
                }

                // Close the connection
                conn.Close();
            }
        }

        public async Task<string> BuildCommandQueryForTransactions(string blockHex, string query, List<TransactionInfoModel> transactionInfoModels)
        {
            List<string> Rows = new List<string>();
            StringBuilder sCommand = new StringBuilder("INSERT INTO transactions (blockID, hash, `from`, `to`, " +
                "value, gas, gasPrice, transactionIndex) VALUES ");

            foreach (var transactionBlock in transactionInfoModels)
            {
                _logger.LogInformation($"[{CommonHelper.FormatDateTimeToLongString()}]: Process to insert transaction index '{transactionBlock?.TransactionIndex}'");
                TransactionsModel transactionModel = await _etherscanService.GetTransactionByBlockNumberAndIndex(blockHex, transactionBlock?.TransactionIndex);

                // Insert the transaction record into the database
                query = _etherscanService.InsertTransactions(transactionModel.Result);
                Rows.Add(query);
            }
            // Append the text into the query
            sCommand.Append(string.Join(",", Rows));
            sCommand.Append(",");

            // Execute the query
            string commandText = sCommand.ToString().Substring(0, sCommand.Length - 1);

            return commandText;
        }
    }
}
