namespace Investments.Infrastructure.Kafka
{
    public class KafkaTopics
    {
        // Tópico para eventos de compra de investimento
        public const string InvestmentPurchasedTopic = "investment-purchased";

        // Tópico para eventos de venda de investimento
        public const string InvestmentSoldTopic = "investment-sold";

        // Tópico para eventos de atualização de produtos financeiros
        public const string InsertProductTopic = "product-inserted";
        public const string UpdateProductTopic = "product-updated";
        public const string DeleteProductTopic = "product-deleted";

        // Tópico para notificações de vencimento de produtos financeiros
        public const string ProductExpiryNotificationTopic = "product-expiry-notification";

        public const string InsertCustomerPorftolioStatement = "insert-customer-portfolio-statement";
        public const string InsertCustomerPorftolio = "update-customer-portfolio";
        public const string DeleteCustomerPorftolio = "delete-customer-portfolio";
    }
}
