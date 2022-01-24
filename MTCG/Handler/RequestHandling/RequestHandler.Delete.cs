using MTCG.Models;
using ServerModule.Mapping;
using ServerModule.Responses;

namespace MTCG.Handler.RequestHandling
{
    public partial class RequestHandler
    {
        private static Response DeleteTradings(RequestData requestData)
        {
            string username = requestData.Authentication.Username;
            string storeId = requestData.PathVariable;
            if (username is null || storeId is null) return Response.Status(Status.BadRequest);
            TradingDeal dealCard = DataHandler.GetCardOfStore(storeId);
            if (dealCard == null) return Response.Status(Status.BadRequest);
            if (DataHandler.GetCard(dealCard.CardToTrade).Username != username) return Response.Status(Status.Forbidden);
            return DataHandler.DeleteTradingDeal(storeId) ? Response.PlainText("Trade deleted successfully") : Response.Status(Status.InternalServerError);
        }
    }
}